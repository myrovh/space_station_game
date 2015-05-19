using UnityEngine;
using UnityEditor;

[InitializeOnLoad]
public class OnUnityLoad
{

	static OnUnityLoad()
	{

		EditorApplication.playmodeStateChanged = () =>
		{

			if( EditorApplication.isPlayingOrWillChangePlaymode && !EditorApplication.isPlaying )
			{

				Debug.Log( "Auto-Saving scene before entering Play mode: " + EditorApplication.currentScene );

				EditorApplication.SaveScene();
				EditorApplication.SaveAssets();
			}

		};

	}

}