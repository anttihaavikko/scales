using UnityEngine;

namespace AnttiStarterKit.Managers
{
	public class AutoEnd : MonoBehaviour {

		private ParticleSystem ps;

		public int Pool { get; set; }

		public void Start() 
		{
			ps = GetComponent<ParticleSystem>();
		}

		public void Update() 
		{
			if(ps)
			{
				if(!ps.IsAlive())
				{
					EffectManager.Instance.ReturnToPool(this);
				}
			}
		}

		public ParticleSystem GetParticleSystem()
		{
			return ps;
		}
	}
}
