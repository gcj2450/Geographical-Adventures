using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TerrainGeneration;

public class LoadingManager_Custom : MonoBehaviour
{
	[Header("References")]
	public TerrainHeightSettings heightSettings;
	public TerrainHeightProcessor heightProcessor;
	public CityLights cityLights;
	public WorldLookup worldLookup;
	public Light sunLight;
	public AtmosphereEffect atmosphereEffect;

	public LodMeshLoader terrainLoader;
	public MeshLoader oceanLoader;
	public MeshLoader countryOutlineLoader;

	// Called before all other scripts (defined in script execution order settings)
	void Awake()
	{
		Load();

	}

	public LoadTask[] GetTasks()
	{
		List<LoadTask> tasks = new List<LoadTask>();

		AddTask(() => heightProcessor.ProcessHeightMap(), "Processing Height Map");
		AddTask(() => cityLights.Init(heightProcessor.processedHeightMap, sunLight), "Creating City Lights");
		AddTask(() => worldLookup.Init(heightProcessor.processedHeightMap), "Initializing World Lookup");
		AddTask(() => terrainLoader.Load(), "Loading Terrain Mesh");
		AddTask(() => oceanLoader.Load(), "Loading Ocean Mesh");
		AddTask(() => countryOutlineLoader.Load(), "Loading Country Outlines");

		void AddTask(System.Action task, string name)
		{
			tasks.Add(new LoadTask(task, name));
		}

		return tasks.ToArray();
	}



	void Load()
	{
		var loadTimer = System.Diagnostics.Stopwatch.StartNew();
		//OnLoadStart();
		LoadTask[] tasks = GetTasks();

		foreach (LoadTask task in tasks)
		{
			long taskTime = task.Execute();
			Debug.Log($"{task.taskName}: {taskTime} ms.");
		}

		OnLoadFinish();
		Debug.Log($"Total load duration: {loadTimer.ElapsedMilliseconds} ms.");
	}



	void OnLoadFinish()
	{
		heightProcessor.Release();
		Resources.UnloadUnusedAssets(); // not sure if any good reason to do this (?)

	}

	public class LoadTask
	{
		public System.Action task;
		public string taskName;

		public LoadTask(System.Action task, string name)
		{
			this.task = task;
			this.taskName = name;
		}

		public long Execute()
		{
			var sw = System.Diagnostics.Stopwatch.StartNew();
			task.Invoke();

			
			return sw.ElapsedMilliseconds;
		}
	}

}
