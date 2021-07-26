using UnityEngine;
using System.Collections;

public class TrafficSystemToolTip : MonoBehaviour 
{
	public  bool m_disableOnStart = true;
	void Start () 
	{
		gameObject.SetActive(!m_disableOnStart);
	}
}
