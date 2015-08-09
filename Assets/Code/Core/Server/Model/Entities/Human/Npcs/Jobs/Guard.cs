using Server.Model.Entities;
using Server.Model.Entities.Human;
using Server.Model.Entities.Human.Npcs;
using UnityEngine;
using System.Collections;

public class Guard : aJob {

    public Guard(NPC me, string captainName, float maxDistance) : base(me)
    {
        _maxDistance = maxDistance;
        _captainName = captainName;
    }

    private string _captainName;
    private float _maxDistance;
    private ServerUnit _myCaptain = null;

    private bool _finished = false;

    public override bool Finished
    {
        get { return _finished; }
    }

    public override bool Continue()
    {
        if (_myCaptain == null)
            _myCaptain = (ServerUnit) n.CurrentBranch.ActiveObjectsVisible.Find(o => o != null && (o as ServerUnit).name == _captainName);
        if (_myCaptain == null)
        {
            if(Random.Range(0,100) > 98)
                n.Speak("Where the fuck is my captain?");
            n.Movement.WalkTo(
                    n.StaticPosition + new Vector3(Random.Range(-4f, 4f), 0, Random.Range(-4f, 4f)), null);
            return false;
        }
        else
        {
            float distance = Vector3.Distance(_myCaptain.Movement.Position, n.Movement.Position);
            if (distance < _maxDistance)
            {
                _finished = true;
                return true;
            }
            else
            {
                n.Movement.WalkTo(
                    _myCaptain.Movement.Position + new Vector3(Random.Range(- 4f, 4f), 0, Random.Range(-4f, 4f)), null);
                return true;
            }
        }
    }
}
