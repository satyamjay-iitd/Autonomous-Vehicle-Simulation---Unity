using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/*[System.Serializable]*/
public class TrafficSystemLane
{
	/*[SerializeField]*/ public  List<Vector3>     m_path                 = new List<Vector3>();
	/*[SerializeField]*/ public  bool              m_pathIsRelative       = false;
}
