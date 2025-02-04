﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

namespace DailyRewardSystem {
	public enum RewardType {
		Coins
	}

	[Serializable] public struct Reward {
		public RewardType Type;
		public int Amount;
	}

	public class DailyRewards : MonoBehaviour {

		[Header ( "Main Menu UI" )]
		
		[SerializeField] Text coinsText;
		

		[Space]
		[Header ( "Reward UI" )]
		[SerializeField] GameObject rewardsCanvas;
		[SerializeField] Button openButton;
		[SerializeField] Button closeButton;
		[SerializeField] Image rewardImage;
		[SerializeField] Text rewardAmountText;
		[SerializeField] Button claimButton;
		[SerializeField] GameObject rewardsNotification;
		[SerializeField] GameObject noMoreRewardsPanel;


		[Space]
		[Header ( "Rewards Images" )]
		
		[SerializeField] Sprite iconCoinsSprite;
		

		[Space]
		[Header ( "Rewards Database" )]
		[SerializeField] RewardsDatabase rewardsDB;

		[Space]
		[Header ( "FX" )]
		
		[SerializeField] ParticleSystem fxCoins;
		

		[Space]
		[Header ( "Timing" )]
		//wait 23 Hours to activate the next reward (it's better to use 23h instead of 24h)
		[SerializeField] double nextRewardDelay = 23f;
		//check if reward is available every 5 seconds
		[SerializeField] float checkForRewardDelay = 5f;


		private int nextRewardIndex;
		private bool isRewardReady = false;

		void Start ( ) {
			Initialize ( );

			StopAllCoroutines ( );
			StartCoroutine ( CheckForRewards ( ) );
		}

		void Initialize ( ) {
			nextRewardIndex = PlayerPrefs.GetInt ( "Next_Reward_Index", 0 );

			//Update Mainmenu UI (metals,coins,gems)
			
			UpdateCoinsTextUI ( );
			

			//Add Click Events
			openButton.onClick.RemoveAllListeners ( );
			openButton.onClick.AddListener ( OnOpenButtonClick );

			closeButton.onClick.RemoveAllListeners ( );
			closeButton.onClick.AddListener ( OnCloseButtonClick );

			claimButton.onClick.RemoveAllListeners ( );
			claimButton.onClick.AddListener ( OnClaimButtonClick );

			//Check if the game is opened for the first time then set Reward_Claim_Datetime to the current datetime
			if ( string.IsNullOrEmpty ( PlayerPrefs.GetString ( "Reward_Claim_Datetime" ) ) )
				PlayerPrefs.SetString ( "Reward_Claim_Datetime", DateTime.Now.ToString ( ) );
		}

		IEnumerator CheckForRewards ( ) {
			while ( true ) {
				if ( !isRewardReady ) {
					DateTime currentDatetime = DateTime.Now;
					DateTime rewardClaimDatetime = DateTime.Parse ( PlayerPrefs.GetString ( "Reward_Claim_Datetime", currentDatetime.ToString ( ) ) );

					//get total Hours between this 2 dates
					double elapsedHours = (currentDatetime - rewardClaimDatetime).TotalHours;

					if ( elapsedHours >= nextRewardDelay )
						ActivateReward ( );
					else
						DesactivateReward ( );
				}

				yield return new WaitForSeconds ( checkForRewardDelay );
			}
		}

		void ActivateReward ( ) {
			isRewardReady = true;

			noMoreRewardsPanel.SetActive ( false );
			rewardsNotification.SetActive ( true );

			//Update Reward UI
			Reward reward = rewardsDB.GetReward ( nextRewardIndex );

			//Icon
			
			if ( reward.Type == RewardType.Coins )
				rewardImage.sprite = iconCoinsSprite;
			

			//Amount
			rewardAmountText.text = string.Format ( "+{0}", reward.Amount );

		}

		void DesactivateReward ( ) {
			isRewardReady = false;

			noMoreRewardsPanel.SetActive ( true );
			rewardsNotification.SetActive ( false );
		}

		void OnClaimButtonClick ( ) {
			Reward reward = rewardsDB.GetReward ( nextRewardIndex );

			//check reward type
			if ( reward.Type == RewardType.Coins ) {
				Debug.Log ( "<color=yellow>" + reward.Type.ToString ( ) + " Claimed : </color>+" + reward.Amount );
				GameData.Coins += reward.Amount;
				fxCoins.Play ( );
				UpdateCoinsTextUI ( );

			} 
			//Save next reward index
			nextRewardIndex++;
			if ( nextRewardIndex >= rewardsDB.rewardsCount )
				nextRewardIndex = 0;

			PlayerPrefs.SetInt ( "Next_Reward_Index", nextRewardIndex );

			//Save DateTime of the last Claim Click
			PlayerPrefs.SetString ( "Reward_Claim_Datetime", DateTime.Now.ToString ( ) );

			DesactivateReward ( );
		}

		//Update Mainmenu UI (metals,coins,gems)--------------------------------
		
		void UpdateCoinsTextUI ( ) {
			coinsText.text = GameData.Coins.ToString ( );
		}

		

		//Open | Close UI -------------------------------------------------------
		void OnOpenButtonClick ( ) {
			rewardsCanvas.SetActive ( true );
		}

		void OnCloseButtonClick ( ) {
			rewardsCanvas.SetActive ( false );
		}
	}

}

