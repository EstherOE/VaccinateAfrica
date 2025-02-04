﻿using UnityEngine;

public static class GameData {
	
	private static int _coins = 0;
	

	//Static Constructor to load data from playerPrefs
	static GameData ( ) {
		
		_coins = PlayerPrefs.GetInt ( "Coins", 0 );
		
	}

	

	public static int Coins {
		get{ return _coins; }
		set{ PlayerPrefs.SetInt ( "Coins", (_coins = value) ); }
	}

	
	/*---------------------------------------------------------
		this line:
		set{ PlayerPrefs.SetInt ( "Gems", (_gems = value) ); }

		is equivalent to:
		set{ 
			_gems = value;
			PlayerPrefs.SetInt ( "Gems", _gems ); 
		}
	------------------------------------------------------------*/
}
