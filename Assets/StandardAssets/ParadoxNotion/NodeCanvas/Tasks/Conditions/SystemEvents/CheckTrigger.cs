using NodeCanvas.Framework;
using ParadoxNotion;
using ParadoxNotion.Design;
using UnityEngine;


namespace NodeCanvas.Tasks.Conditions{

	[Category("System Events")]
	[EventReceiver("OnTriggerEnter", "OnTriggerExit")]
	public class CheckTrigger : ConditionTask<Collider> {

		public TriggerTypes checkType = TriggerTypes.TriggerEnter;
        public LayerMask layerMask;
		public bool specifiedTagOnly;
		[TagField]
		public string objectTag = "Untagged";
		[BlackboardOnly]
		public BBParameter<GameObject> saveGameObjectAs;

		private bool stay;
        private Collider collider;

		protected override string info{
			get {return checkType.ToString() + ( specifiedTagOnly? (" '" + objectTag + "' tag") : "" );}
		}

		protected override bool OnCheck(){
			if (checkType == TriggerTypes.TriggerStay && collider && collider.enabled)
				return stay;
			return false;
		}

		public void OnTriggerEnter(Collider other){
			if ( (((1<<other.gameObject.layer) & layerMask) != 0) && (!specifiedTagOnly || other.gameObject.tag == objectTag))
            {
				stay = true;
                collider = other;

                if (checkType == TriggerTypes.TriggerEnter || checkType == TriggerTypes.TriggerStay){
					saveGameObjectAs.value = other.gameObject;
					YieldReturn(true);
				}
			}
		}

		public void OnTriggerExit(Collider other){
			if ((((1 << other.gameObject.layer) & layerMask) != 0) && (!specifiedTagOnly || other.gameObject.tag == objectTag))
            {
				stay = false;
                collider = null;
                if (checkType == TriggerTypes.TriggerExit){
					saveGameObjectAs.value = other.gameObject;				
					YieldReturn(true);
				}
			}
		}
	}
}