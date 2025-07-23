using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintDirection : MonoBehaviour
{
    LineRenderer m_line;
    private Transform pos1;
    private Transform pos2;
    float _distanceFromHeads = 1;
    float _distanceFromEnds = 0;

    public Transform Pos1
    {
        get => pos1; set
        {
            pos1 = value;
            SetPos();
        }
    }
    public Transform Pos2
    {
        get => pos2; set
        {
            pos2 = value;
            SetPos();
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        m_line = GetComponent<LineRenderer>();
        m_line.positionCount = 2;
    }

    // Update is called once per frame
    void SetPos()
    {
        // m_line = GetComponent<LineRenderer>();
        // m_line.positionCount = 2;
        if (pos1 != null && pos2 != null)
        {
            Vector3 direction = (pos2.position - pos1.position).normalized;
            Vector3 point1 = pos1.position + direction * _distanceFromHeads;
            Vector3 point2 = pos2.position - direction * _distanceFromEnds;

            m_line.SetPosition(0, new Vector3(point1.x, point1.y + 2, point1.z));
            m_line.SetPosition(1, new Vector3(point2.x, point2.y + 2, point2.z));
        }
    }
}
