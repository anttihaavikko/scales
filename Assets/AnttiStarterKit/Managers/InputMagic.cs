using UnityEngine;

namespace AnttiStarterKit.Managers
{
	public class InputMagic : MonoBehaviour {

		public const int A = 0;
		public const int B = 1;
		public const int X = 2;
		public const int Y = 3;
		public const int START = 4;
		public const int SELECT = 5;

		public const int STICK_LEFT_X = 0;
		public const int STICK_LEFT_Y = 1;
		public const int STICK_RIGHT_X = 2;
		public const int STICK_RIGHT_Y = 3;
		public const int DPAD_X = 4;
		public const int DPAD_Y = 5;
		public const int STICK_OR_DPAD_X = 6;
		public const int STICK_OR_DPAD_Y = 7;

		public const int METHOD_KEYBOARD = 0;
		public const int METHOD_PLAYSTATION = 1;
		public const int METHOD_XBOX = 2;

		// Static singleton property
		private static InputMagic instance;

		private string padName = "";
		private string realPadName = "";
		private int currentMethod = 0;

		private string padA = "";
		private string padB = "";
		private string padX = "";
		private string padY = "";

		private string padLeftStickX = "";
		private string padLeftStickY = "";

		private string padRightStickX = "";
		private string padRightStickY = "";

		private string padDPadX = "";
		private string padDPadY = "";
		private int padDPadDirection = 1;

		private string padStart = "";
		private string padSelect = "";

		private int numOfPads = 0;

		// Static singleton property
		public static InputMagic Instance {
			get {
				if (instance) {
					return instance;
				} else {
					instance = new GameObject("InputMagic").AddComponent<InputMagic>();
					instance.FindPad ();
					DontDestroyOnLoad(instance.gameObject);
					return instance;
				}
			}
		}

		void FindPad() {

			currentMethod = METHOD_KEYBOARD;
			realPadName = "";

			if (Input.GetJoystickNames ().Length > 0) {

				padName = Input.GetJoystickNames () [0];

				/****************
			* OSX
			****************/

				if (Application.platform == RuntimePlatform.OSXPlayer || Application.platform == RuntimePlatform.OSXEditor) {

					if (padName == "Sony Computer Entertainment Wireless Controller" || padName == "Unknown Wireless Controller") {

						realPadName = "PS4 Controller";

						padA = "joystick 1 button 1";
						padB = "joystick 1 button 2";
						padX = "joystick 1 button 0";
						padY = "joystick 1 button 3";

						padLeftStickX = "joystick 1 analog 0";
						padLeftStickY = "joystick 1 analog 1";

						padRightStickX = "joystick 1 analog 2";
						padRightStickY = "joystick 1 analog 3";

						padDPadX = "joystick 1 analog 6";
						padDPadY = "joystick 1 analog 7";
						padDPadDirection = 1;

						padStart = "joystick 1 button 9";
						padSelect = "joystick 1 button 8";

						currentMethod = METHOD_PLAYSTATION;

						Debug.Log ("Pad configured!");

						return;
					}

					if (padName == "Sony PLAYSTATION(R)3 Controller") {

						realPadName = "PS3 Controller";

						padA = "joystick 1 button 14";
						padB = "joystick 1 button 13";
						padX = "joystick 1 button 15";
						padY = "joystick 1 button 12";

						padLeftStickX = "joystick 1 analog 0";
						padLeftStickY = "joystick 1 analog 1";

						padRightStickX = "joystick 1 analog 2";
						padRightStickY = "joystick 1 analog 3";

						padDPadX = "joystick 1 analog 6";
						padDPadY = "joystick 1 analog 7";
						padDPadDirection = 1;

						padStart = "joystick 1 button 3";
						padSelect = "joystick 1 button 0";

						currentMethod = METHOD_PLAYSTATION;

						Debug.Log ("Pad configured!");

						return;
					}

					if (padName.ToLower().IndexOf ("360") != -1 || padName.ToLower().IndexOf ("xbox") != -1 || padName.ToLower().IndexOf ("catz") != -1) {

						realPadName = "XBOX One/360 Controller";

						padA = "joystick 1 button 16";
						padB = "joystick 1 button 17";
						padX = "joystick 1 button 18";
						padY = "joystick 1 button 19";

						padLeftStickX = "joystick 1 analog 0";
						padLeftStickY = "joystick 1 analog 1";

						padRightStickX = "joystick 1 analog 2";
						padRightStickY = "joystick 1 analog 3";

						padDPadX = "";
						padDPadY = "";
						padDPadDirection = -1;

						padStart = "joystick 1 button 9";
						padSelect = "joystick 1 button 10";

						currentMethod = METHOD_XBOX;

						Debug.Log ("Pad configured!");

						return;
					}
				}

				/****************
			* Windows
			****************/

				if (Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor) {

					if (padName == "Wireless Controller") {

						realPadName = "PS4 Controller";

						padA = "joystick 1 button 1";
						padB = "joystick 1 button 2";
						padX = "joystick 1 button 0";
						padY = "joystick 1 button 3";

						padLeftStickX = "joystick 1 analog 0";
						padLeftStickY = "joystick 1 analog 1";

						padRightStickX = "joystick 1 analog 2";
						padRightStickY = "joystick 1 analog 5";

						padDPadX = "joystick 1 analog 6";
						padDPadY = "joystick 1 analog 7";
						padDPadDirection = -1;

						padStart = "joystick 1 button 9";
						padSelect = "joystick 1 button 8";

						currentMethod = METHOD_PLAYSTATION;

						Debug.Log ("Pad configured!");

						return;
					}

					if (padName == "MotioninJoy Virtual Game Controller") {

						realPadName = "PS3 Controller";

						padA = "joystick 1 button 2";
						padB = "joystick 1 button 1";
						padX = "joystick 1 button 3";
						padY = "joystick 1 button 0";

						padLeftStickX = "joystick 1 analog 0";
						padLeftStickY = "joystick 1 analog 1";

						padRightStickX = "joystick 1 analog 2";
						padRightStickY = "joystick 1 analog 5";

						padDPadX = "joystick 1 analog 8";
						padDPadY = "joystick 1 analog 9";
						padDPadDirection = 1;

						padStart = "joystick 1 button 11";
						padSelect = "joystick 1 button 8";

						currentMethod = METHOD_PLAYSTATION;

						Debug.Log ("Pad configured!");

						return;
					}

					if (padName.ToLower().IndexOf ("360") != -1 || padName.ToLower().IndexOf ("xbox") != -1 || padName.ToLower().IndexOf ("catz") != -1) {

						realPadName = "XBOX One/360 Controller";

						padA = "joystick 1 button 0";
						padB = "joystick 1 button 1";
						padX = "joystick 1 button 2";
						padY = "joystick 1 button 3";

						padLeftStickX = "joystick 1 analog 0";
						padLeftStickY = "joystick 1 analog 1";

						padRightStickX = "joystick 1 analog 3";
						padRightStickY = "joystick 1 analog 4";

						padDPadX = "joystick 1 analog 5";
						padDPadY = "joystick 1 analog 6";
						padDPadDirection = -1;

						padStart = "joystick 1 button 7";
						padSelect = "joystick 1 button 6";

						currentMethod = METHOD_XBOX;

						Debug.Log ("Pad configured!");

						return;
					}
				}

				/****************
			* Linux
			****************/

				if (Application.platform == RuntimePlatform.LinuxPlayer || Application.platform == RuntimePlatform.LinuxEditor) {

					if (padName == "Sony Computer Entertainment Wireless Controller") {

						realPadName = "PS4 Controller";

						padA = "joystick 1 button 1";
						padB = "joystick 1 button 2";
						padX = "joystick 1 button 0";
						padY = "joystick 1 button 3";

						padLeftStickX = "joystick 1 analog 0";
						padLeftStickY = "joystick 1 analog 1";

						padRightStickX = "joystick 1 analog 2";
						padRightStickY = "joystick 1 analog 5";

						padDPadX = "joystick 1 analog 6";
						padDPadY = "joystick 1 analog 7";
						padDPadDirection = 1;

						padStart = "joystick 1 button 9";
						padSelect = "joystick 1 button 8";

						currentMethod = METHOD_PLAYSTATION;

						Debug.Log ("Pad configured!");

						return;
					}

					if (padName.ToLower ().IndexOf ("PLAYSTATION(R)3") != -1) {

						realPadName = "PS3 Controller";

						padA = "joystick 1 button 14";
						padB = "joystick 1 button 13";
						padX = "joystick 1 button 15";
						padY = "joystick 1 button 12";

						padLeftStickX = "joystick 1 analog 0";
						padLeftStickY = "joystick 1 analog 1";

						padRightStickX = "joystick 1 analog 2";
						padRightStickY = "joystick 1 analog 3";

						padDPadX = "joystick 1 analog 6";
						padDPadY = "joystick 1 analog 7";
						padDPadDirection = 1;

						padStart = "joystick 1 button 3";
						padSelect = "joystick 1 button 0";

						currentMethod = METHOD_PLAYSTATION;

						Debug.Log ("Pad configured!");

						return;
					}

					if (padName.ToLower ().IndexOf ("360") != -1 || padName.ToLower ().IndexOf ("xbox") != -1 || padName.ToLower ().IndexOf ("catz") != -1) {

						realPadName = "XBOX One/360 Controller";

						padA = "joystick 1 button 0";
						padB = "joystick 1 button 1";
						padX = "joystick 1 button 2";
						padY = "joystick 1 button 3";

						padLeftStickX = "joystick 1 analog 0";
						padLeftStickY = "joystick 1 analog 1";

						padRightStickX = "joystick 1 analog 3";
						padRightStickY = "joystick 1 analog 4";

						padDPadX = "joystick 1 analog 6";
						padDPadY = "joystick 1 analog 7";
						padDPadDirection = 1;

						padStart = "joystick 1 button 7";
						padSelect = "joystick 1 button 6";

						currentMethod = METHOD_XBOX;

						Debug.Log ("Pad configured!");

						return;
					}
				}

				/****************
			* WebGL
			****************/

				if (Application.platform == RuntimePlatform.WebGLPlayer) {

					// ps4 (ps3?) chrome
					if (padName.ToLower().IndexOf ("054c") != -1) {

						realPadName = "PS4/PS3 Controller";

						padA = "joystick 1 button 0";
						padB = "joystick 1 button 1";
						padX = "joystick 1 button 2";
						padY = "joystick 1 button 3";

						padLeftStickX = "joystick 1 analog 0";
						padLeftStickY = "joystick 1 analog 1";

						padRightStickX = "joystick 1 analog 3";
						padRightStickY = "joystick 1 analog 4";

						padDPadX = "joystick 1 analog 5";
						padDPadY = "joystick 1 analog 6";
						padDPadDirection = -1;

						padStart = "joystick 1 button 7";
						padSelect = "joystick 1 button 6";

						currentMethod = METHOD_PLAYSTATION;

						Debug.Log ("Pad configured!");

						return;
					}

					// ps4 (ps3?) firefox
					if (padName.ToLower().IndexOf ("54c") != -1) {

						realPadName = "PS4/PS3 Controller";

						padA = "joystick 1 button 1";
						padB = "joystick 1 button 2";
						padX = "joystick 1 button 0";
						padY = "joystick 1 button 3";

						padLeftStickX = "joystick 1 analog 0";
						padLeftStickY = "joystick 1 analog 1";

						padRightStickX = "joystick 1 analog 2";
						padRightStickY = "joystick 1 analog 5";

						padDPadX = "joystick 1 analog 6";
						padDPadY = "joystick 1 analog 7";
						padDPadDirection = -1;

						padStart = "joystick 1 button 9";
						padSelect = "joystick 1 button 8";

						currentMethod = METHOD_PLAYSTATION;

						Debug.Log ("Pad configured!");

						return;
					}

					// 360?
					if (padName.IndexOf ("0x045E") != -1 || padName.ToLower().IndexOf ("xinput") != -1) {

						realPadName = "XBOX One/360 Controller";

						padA = "joystick 1 button 0";
						padB = "joystick 1 button 1";
						padX = "joystick 1 button 2";
						padY = "joystick 1 button 3";

						padLeftStickX = "joystick 1 analog 0";
						padLeftStickY = "joystick 1 analog 1";

						padRightStickX = "joystick 1 analog 2";
						padRightStickY = "joystick 1 analog 3";

						padDPadX = "joystick 1 analog 5";
						padDPadY = "joystick 1 analog 6";
						padDPadDirection = -1;

						padStart = "joystick 1 button 9";
						padSelect = "joystick 1 button 8";

						currentMethod = METHOD_XBOX;

						Debug.Log ("Pad configured!");

						return;
					}
				}

				Debug.Log ("Unknown pad: " + padName);
				realPadName = "";

			}
		}

		void Update() {

			if(Input.GetJoystickNames ().Length != numOfPads) {
				Debug.Log ("Pad count changed...");
				numOfPads = Input.GetJoystickNames ().Length;
				FindPad();
			}

		}

		public int GetInputMethod() {
			return currentMethod;
		}

		public string GetPadName() {
			return realPadName;
		}

		public bool GetButtonDown(int button) {

			if (button == InputMagic.A) {
				return TryToGetKeyDown (padA) || Input.GetKeyDown(KeyCode.Space);
			}
			if (button == InputMagic.B) {
				return TryToGetKeyDown (padB) || Input.GetKeyDown(KeyCode.X);
			}
			if (button == InputMagic.X) {
				return TryToGetKeyDown (padX) || Input.GetKeyDown(KeyCode.Z);
			}
			if (button == InputMagic.Y) {
				return TryToGetKeyDown (padY) || Input.GetKeyDown(KeyCode.C);
			}
			if (button == InputMagic.START) {
				return TryToGetKeyDown (padStart);
			}
			if (button == InputMagic.SELECT) {
				return TryToGetKeyDown (padSelect);
			}

			return false;
		}

		public bool GetButton(int button) {

			if (button == InputMagic.A) {
				return TryToGetKey (padA) || Input.GetKey(KeyCode.Space);
			}
			if (button == InputMagic.B) {
				return TryToGetKey (padB) || Input.GetKey(KeyCode.X);
			}
			if (button == InputMagic.X) {
				return TryToGetKey (padX) || Input.GetKey(KeyCode.Z);
			}
			if (button == InputMagic.Y) {
				return TryToGetKey (padY) || Input.GetKey(KeyCode.C);
			}
			if (button == InputMagic.START) {
				return TryToGetKey (padStart);
			}
			if (button == InputMagic.SELECT) {
				return TryToGetKey (padSelect);
			}

			return false;
		}

		public bool GetButtonUp(int button) {

			if (button == InputMagic.A) {
				return TryToGetKeyUp (padA) || Input.GetKeyUp(KeyCode.Space);
			}
			if (button == InputMagic.B) {
				return TryToGetKeyUp (padB) || Input.GetKeyUp(KeyCode.X);
			}
			if (button == InputMagic.X) {
				return TryToGetKeyUp (padX) || Input.GetKeyUp(KeyCode.Z);
			}
			if (button == InputMagic.Y) {
				return TryToGetKeyUp (padY) || Input.GetKeyUp(KeyCode.C);
			}
			if (button == InputMagic.START) {
				return TryToGetKeyUp (padStart);
			}
			if (button == InputMagic.SELECT) {
				return TryToGetKeyUp (padSelect);
			}

			return false;
		}

		private bool TryToGetKey(string button) {
			if(button != "") {
				return Input.GetKey (button);
			}
			return false;
		}

		private bool TryToGetKeyDown(string button) {
			if(button != "") {
				return Input.GetKeyDown (button);
			}
			return false;
		}

		private bool TryToGetKeyUp(string button) {
			if(button != "") {
				return Input.GetKeyUp (button);
			}
			return false;
		}

		public float GetAxis(int axis) {

			if (axis == InputMagic.STICK_LEFT_X) {
				return Mathf.Clamp(TryToGetAxis (padLeftStickX) + Input.GetAxisRaw ("Horizontal"), -1f, 1f);
			}
			if (axis == InputMagic.STICK_LEFT_Y) {
				return Mathf.Clamp(-TryToGetAxis (padLeftStickY) + Input.GetAxisRaw ("Vertical"), -1f, 1f);
			}
			if (axis == InputMagic.STICK_RIGHT_X) {
				return TryToGetAxis (padRightStickX);
			}
			if (axis == InputMagic.STICK_RIGHT_Y) {
				return -TryToGetAxis (padRightStickY);
			}
			if (axis == InputMagic.DPAD_X) {
				return TryToGetAxis (padDPadX);
			}
			if (axis == InputMagic.DPAD_Y) {
				return -TryToGetAxis (padDPadY) * padDPadDirection;
			}
			if (axis == InputMagic.STICK_OR_DPAD_X) {
				return Mathf.Clamp(TryToGetAxis (padLeftStickX) + TryToGetAxis (padDPadX) + Input.GetAxisRaw ("Horizontal"), -1f, 1f);
			}
			if (axis == InputMagic.STICK_OR_DPAD_Y) {
				return Mathf.Clamp(-TryToGetAxis (padLeftStickY) + -TryToGetAxis (padDPadY) * padDPadDirection + Input.GetAxisRaw ("Vertical"), -1f, 1f);
			}


			return 0;
		}

		private float TryToGetAxis(string axis) {
			if(axis != "") {
				return Input.GetAxisRaw (axis);
			}
			return 0;
		}
	}
}
