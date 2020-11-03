using System.Collections.Generic;
using Core.EventSystem;
using NaughtyAttributes;
using UnityEngine;

public class LevelCounter : MessageListener<BoardEvent>
{
    /*
     * lines for level up (current level * 10)
    */


    //////////////////////////////////////////////////////////////////////////
    [SerializeField, ReadOnly]
    private int                     m_Lines;
    public int                      Lines => m_Lines;
    public float                    Progress => _GetProgress();
    
    [SerializeField]
    private int                     m_Level;
    [SerializeField]
    private LevelData               m_LevelData;
    public int                      Level
    {
        set
        {
            //if (value == m_Level)
            //    return;

            var exchange = Mathf.Max(0, value) - m_Level;

            m_Level += exchange;
            var levelIndex = Mathf.Clamp(m_LevelData.DataList.Count - 1, 0, m_Level);

            // update lines count
            m_Lines = _GetLines(m_Level);

            // save inspector value
            TetrisManager.Instance.StepInterval = m_LevelData.DataList[levelIndex].GravityPerSecond;

            // send level exchange event
            MessageSystem.Send(BoardEvent.Level, exchange);
        }
        get => m_Level;
    }

    //////////////////////////////////////////////////////////////////////////
    private void Start()
    {
        m_Lines = _GetLines(m_Level);
    }

    private int _GetLines(int level)
    {
        var lines = 0;
        for (var n = 1; n <= level; n++)
            lines += n * 10;

        return lines;
    }

    public override void ProcessMessage(IMessage<BoardEvent> e)
    {
        switch (e.Key)
        {
            case BoardEvent.CollapseRows:
            {
                // update level
                var rows = e.GetData<List<int>>().Count;
                m_Lines += rows;

                var level = _GetLevel();
                if (level != Level)
                    Level = level;
            } break;
        }
    }

    [Button]
    public void IncreaseLevel()
    {
        Level ++;
    }

    //////////////////////////////////////////////////////////////////////////
    private int _GetLevel()
    {
        // ugly math
        var level = 0;
        var lines = m_Lines;

        do
        {
            level ++;
            lines -= level * 10;
        }
        while (lines >= 0);

        return level - 1;
    }

    private float _GetProgress()
    {
        var level = 0;
        var lines = m_Lines;
        var levelCost = 0;
        do
        {
            level ++;
            levelCost = level * 10;
            lines -= levelCost;
        }
        while (lines >= 0);

        return (lines + levelCost) / (float)levelCost;
    }
}