using Unity.VisualScripting;
using UnityEngine;

public class DnDRäkning : MonoBehaviour
{
    public int UserSkill;
    public int UserDmg;
    public int T20;
    public int T6;
    public int T100;
    void Start()
    {
        //Roll the userskill from 1-20
        UserSkill = Random.Range(1, 20);
        //Roll the users damage from 1-6
        UserDmg = Random.Range(1, 6);
    }

    // Update is called once per frame
    void Update()
    {
        //Om man klicklar på musknappen
        if (Input.GetMouseButtonDown(0))
        {
            //Roll the dice
            T20 = Random.Range(1, 20);
            T6 = Random.Range(1, 6);

            if(T20 == 20)
            {
                Debug.Log("Du fumla");
            }
            if(T20 == 1)
            {
                Debug.Log("Du träffa exeptionellt");
                if(T6 == 6)
                {
                    Debug.Log("Du gjorde max skada" + UserDmg * T6);
                }
                else
                {
                    Debug.Log("Du gjorde " + UserDmg * T6 + " skada");
                }
            }
            if(T20 >= UserSkill)
            {
                //If the dice is higher than the userskill
                Debug.Log("Du missade");
            }
            else
            {
                Debug.Log("Du träffade");

            }
        }
    }


}
