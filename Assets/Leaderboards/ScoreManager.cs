using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace Leaderboards
{
	[Serializable]
	public class LeaderBoardData
	{
		public LeaderBoardScore[] scores;
	}

	[Serializable]
	public class LeaderBoardScore {
		public string name;
		public string score;
		public string level;
		public int position;
		public string pid;
		public string locale;

		public static LeaderBoardScore CreateFromJSON(string jsonString) {
			return JsonUtility.FromJson<LeaderBoardScore>(jsonString);
		}
	}

	public class ScoreManager : MonoBehaviour {

		public string gameName;
		public int perPage = 5;
		public Action onUploaded, onLoaded;
		public Action<int> onRankFound;

		private const string SaveURL = "https://games.sahaqiel.com/leaderboards/save-score.php?str=";
		private const string LoadURL = "https://games.sahaqiel.com/leaderboards/load-scores-vball.php";

		public bool EndReached { get; private set; }
		
		private CertificateHandler certHandler;

		/******/

		private static ScoreManager instance = null;
		private LeaderBoardData data;

		public static ScoreManager Instance {
			get { return instance; }
		}

		public void CancelLeaderboards() {
			StopAllCoroutines ();
		}

		public void LoadLeaderBoards(int p) {
			StartCoroutine(DoLoadLeaderBoards(p));
		}

		public LeaderBoardData GetData()
		{
			return data;
		}

		private IEnumerator DoLoadLeaderBoards(int p) {
			var www = UnityWebRequest.Get(LoadURL + "?amt=" + perPage + "&p=" + p + "&game=" + gameName);
			www.certificateHandler = certHandler;

			yield return www.SendWebRequest();

			if (string.IsNullOrEmpty (www.error)) {
				data = JsonUtility.FromJson<LeaderBoardData> (www.downloadHandler.text);
				EndReached = data.scores.Length < perPage;
				onLoaded?.Invoke();
			}
		}

		public void FindPlayerRank(string playerName, int score, string id) {
			if (score > 0) {
				StartCoroutine (DoFindPlayerRank (playerName, score, id));
			}
		}

		private IEnumerator DoFindPlayerRank(string playerName, int score, string id) {
			var url = "https://games.sahaqiel.com/leaderboards/get-rank.php?score=" + score + "&name=" + playerName + "&pid=" + SystemInfo.deviceUniqueIdentifier + "&game=" + gameName;
			
			var www = UnityWebRequest.Get(url);
			www.certificateHandler = certHandler;

			yield return www.SendWebRequest();

			if (!string.IsNullOrEmpty(www.error)) yield break;
			
			var localRank = int.Parse (www.downloadHandler.text);
			
			onRankFound?.Invoke(localRank);
		}

		private void Awake() {
			if (instance != null && instance != this) {
				Destroy (this.gameObject);
				return;
			}

			instance = this;
			certHandler = new CustomCertificateHandler();
		}

		public void SubmitScore(string entryName, long scoreSub, long levelSub, string id) {
			var check = (int)Secrets.GetVerificationNumber(entryName, scoreSub, levelSub);
			
			var parameters = "";
			parameters += entryName;
			parameters += "," + id;
			parameters += "," + levelSub;
			parameters += "," + scoreSub;
			parameters += "," + check;
			parameters += "," + gameName;
			
			StartCoroutine(DoSubmitScore(parameters));
		}

		private IEnumerator DoSubmitScore(string parameters) {
			var www = UnityWebRequest.Get(SaveURL + parameters);
			www.certificateHandler = certHandler;

			yield return www.SendWebRequest();

			Invoke (nameof(UploadingDone), 0.75f);
		}

		public void UploadingDone() {
			onUploaded?.Invoke();
		}
	}

	public class CustomCertificateHandler : CertificateHandler
	{
		// Encoded RSAPublicKey
		private static readonly string PUB_KEY = "";


		/// <summary>
		/// Validate the Certificate Against the Amazon public Cert
		/// </summary>
		/// <param name="certificateData">Certifcate to validate</param>
		/// <returns></returns>
		protected override bool ValidateCertificate(byte[] certificateData)
		{
			return true;
		}
	}
}