using System.Collections.Generic;
using Server.Model.Entities;
using Server.Model.Entities.Human;
using Server.Model.Entities.Human.Npcs;
using UnityEngine;
using System.Collections;

public class Troll : aJob
{

    private List<string> _says = new List<string>();

    private float _range = 10f;
    private bool _startedWalking = false;
    private bool _finished = false;
    private bool _interrupted = false;

    #region constructors
    public Troll(NPC me)
        : base(me)
    {
        this._says.Add("trollolol");
    }

    public Troll(NPC me, string s1)
        : base(me)
    {
        this._says.Add(s1);
    }
    public Troll(NPC me, string s1, string s2)
        : base(me)
    {
        this._says.Add(s1);
        this._says.Add(s2);
    }
    public Troll(NPC me, string s1, string s2, string s3)
        : base(me)
    {
        this._says.Add(s1);
        this._says.Add(s2);
        this._says.Add(s3);
    }
    public Troll(NPC me, string s1, string s2, string s3, string s4)
        : base(me)
    {
        this._says.Add(s1);
        this._says.Add(s2);
        this._says.Add(s3);
        this._says.Add(s4);
    }
    public Troll(NPC me, string s1, string s2, string s3, string s4, string s5)
        : base(me)
    {
        this._says.Add(s1);
        this._says.Add(s2);
        this._says.Add(s3);
        this._says.Add(s4);
        this._says.Add(s5);
    }
    public Troll(NPC me, string s1, string s2, string s3, string s4, string s5, string s6)
        : base(me)
    {
        this._says.Add(s1);
        this._says.Add(s2);
        this._says.Add(s3);
        this._says.Add(s4);
        this._says.Add(s5);
        this._says.Add(s6);
    }
    public Troll(NPC me, string s1, string s2, string s3,  string s4, string s5, string s6, string s7)
        : base(me)
    {
        this._says.Add(s1);
        this._says.Add(s2);
        this._says.Add(s3);
        this._says.Add(s4);
        this._says.Add(s5);
        this._says.Add(s6);
        this._says.Add(s7);
    }
    #endregion

    public override bool Finished
    {
        get { return _finished; }
    }

    public override bool Continue()
    {
        if (!_interrupted)
        {
            if (!_startedWalking)
            {
                var target = FindTrollTarget();
                if (target != null)
                {
                    _startedWalking = true;
                    GoTrollTarget(target);
                }
            }
            return true;
        }
        return false;

    }

    private void GoTrollTarget(ServerUnit target)
    {
        float distance = Vector3.Distance(target.Movement.Position, n.Movement.Position);
        if (distance < _range)
        {
            if (distance > 2.5f)
            {
                n.Movement.WalkTo(
                    target.Movement.Position +
                    new Vector3(Random.Range(-_range/4f, _range/4f), 0, Random.Range(-_range/4f, _range/4f)),
                    () => GoTrollTarget(target), () => _interrupted = true);
            }
            else
            {
                n.Focus.FocusedUnit = target;
                n.Movement.RotateWay(target.Movement.Position - n.Movement.Position);
                _finished = true;
                n.Speak(_says[Random.Range(0, _says.Count)].ToString());
            }
        }
        else
        {
            _interrupted = true;
        }
    }

    private ServerUnit FindTrollTarget()
    {
        List<Human> trollableHumans = new List<Human>();
        foreach (var o in n.CurrentBranch.ActiveObjectsVisible)
        {
            var human = o as Human;
            if (human != null && human != n && Vector3.Distance(n.Movement.Position, human.Movement.Position) < _range)
            {
                trollableHumans.Add(human);
            }
        }
        if(trollableHumans.Count > 0)
            return trollableHumans[Random.Range(0, trollableHumans.Count)];
        return null;
    }
}
