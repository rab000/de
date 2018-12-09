
using System.Collections.Generic;
using System;
using UnityEngine;
using System.IO;
public class ActiveSkillDataTemplate
{
    static bool BeShowLog = true;

    #region 主动技能模板
    private static string[,] Content = new string[215, 3]{
		//基本技能数据段
        {"ID",                      "null",      "custom"},              //0默认值为null这里代表自增长
		{"NAME",                    "name",      "custom"},              //1
        {"Content",                 "描述",       "custom"},              //2
		{"SKOperateType",           "0",         "default_SKOperateType"},//3
        {"CanInterrupted",          "false",     "custom"},             //4
        {"InterruptID",             "0",         "custom"},             //5
        {"InterruptedID",           "0",         "custom"},             //6
        {"ColdTime",                "1",         "custom"},             //7

        {"SKPartNum",               "1",         "custom"},             //8
		{"SKLevelNum",              "1",         "custom"},				//9

        //part信息35条-----------------------------------------------------------------
		//SKPart1段
        {"Part1_SwitchType",        "0",         "default_SwitchType"}, //10
        {"Part1_DetectType",        "1",         "default_DetectType"}, //11
        {"Part1_TargetType",        "1",         "default_TargetType"}, //12
        {"Part1_BeMultTarget",      "false",     "custom"},             //13

        {"Part1_AnimResID",         "1",         "artRes_AnimResID"},   //14
        {"Part1_AnimName",          "AnimName",  "custom"},             //15
        {"Part1_AnimSpeedScale",    "1",         "custom"},             //16

        {"Part1_AttackEffID",       "1",         "custom"},             //17
        {"Part1_AnimEvent4AttackEff","1",        "custom"},             //18

        {"Part1_MotionType",        "1",         "default_MotionType"}, //19
        {"Part1_MotionSpeed",       "1",         "custom"},             //20
        {"Part1_MotionMaxDis",      "1",         "custom"},             //21

        {"Part1_CollideType",       "1",         "default_CollideType"},//22
        {"Part1_CollideRectW",      "1",         "custom"},             //23
        {"Part1_CollideRectH",      "1",         "custom"},             //24
        {"Part1_CollideCircleR",    "1",         "custom"},             //25
        {"Part1_CollideSectorAngle","1",         "custom"},             //26
        {"Part1_AnimEvent4Collide", "1",         "custom"},             //27

        {"Part1_CreateObjID",       "0",         "custom"},             //28
        {"Part1_AnimEvent4CreateObj","1",        "custom"},             //29

        {"Part1_BuffID",            "0",         "custom"},             //30
        {"Part1_AnimEvent4Buff",    "1",         "custom"},             //31

        {"Part1_EnemyHurtType",     "1",         "default_EnemyHurtType"},//32
        {"Part1_EnemyHurtValue",    "1",         "custom"},             //33
        {"Part1_EnemyHurtEffID",    "0",         "custom"},             //34
        {"Part1_EnemyHurtState",    "0",         "default_EnemyHurtState"},//35
        {"Part1_EnemyHurtBuffID",   "0",         "custom"},             //36
        {"Part1_EnemyHurtCreateObjID","0",       "custom"},             //37
        {"Part1_EnemyHurtLinkType", "0",         "custom"},             //38

        {"Part1_SelfFeedbackType",   "0",        "custom"},             //39
        {"Part1_SelfFeedbackEffID",  "0",        "custom"},             //40
        {"Part1_SelfFeedbackState",  "0",        "default_SelfFeedbackState"},//41
        {"Part1_SelfFeedbackBuffID", "0",        "custom"},             //42
        {"Part1_SelfFeedbackCreateObjID","0",    "custom"},             //43

        {"Part1_MultSkillTime",     "1",         "custom"},             //44
         //SKPart2段-----------------------------------------------------------------
        {"Part2_SwitchType",        "0",         "default_SwitchType"}, //45
        {"Part2_DetectType",        "1",         "default_DetectType"}, //46 
        {"Part2_TargetType",        "1",         "default_TargetType"}, //47
		{"Part2_BeMultTarget",      "false",      "custom"},             //48

        {"Part2_AnimResID",         "1",         "artRes_AnimResID"},   //49
        {"Part2_AnimName",          "AnimName",  "custom"},//50
        {"Part2_AnimSpeedScale",    "1",         "custom"},//51

        {"Part2_AttackEffID",       "1",         "custom"},//52
        {"Part2_AnimEvent4AttackEff","1",        "custom"},//53

        {"Part2_MotionType",        "1",         "default_MotionType"},//54
        {"Part2_MotionSpeed",       "1",         "custom"},//55
        {"Part2_MotionMaxDis",      "1",         "custom"},//56

        {"Part2_CollideType",       "1",         "default_CollideType"},//57
        {"Part2_CollideRectW",      "1",         "custom"},//58
        {"Part2_CollideRectH",      "1",         "custom"},//59
        {"Part2_CollideCircleR",    "1",         "custom"},//60
        {"Part2_CollideSectorAngle","1",         "custom"},//61
        {"Part2_AnimEvent4Collide", "1",         "custom"},//62

        {"Part2_CreateObjID",       "0",         "custom"},//63
        {"Part2_AnimEvent4CreateObj","1",        "custom"},//64

        {"Part2_BuffID",            "0",         "custom"},//65
        {"Part2_AnimEvent4Buff",    "1",         "custom"},//66

        {"Part2_EnemyHurtType",     "1",         "default_EnemyHurtType"},//67
        {"Part2_EnemyHurtValue",    "1",         "custom"},//68
        {"Part2_EnemyHurtEffID",    "0",         "custom"},//69
        {"Part2_EnemyHurtState",    "0",         "default_EnemyHurtState"},//70
        {"Part2_EnemyHurtBuffID",   "0",         "custom"},//71
        {"Part2_EnemyHurtCreateObjID","0",       "custom"},//72
        {"Part2_EnemyHurtLinkType", "0",         "custom"},//73

        {"Part2_SelfFeedbackType",   "0",        "custom"},//74
        {"Part2_SelfFeedbackEffID",  "0",        "custom"},//75
        {"Part2_SelfFeedbackState",  "0",        "default_SelfFeedbackState"},//76
        {"Part2_SelfFeedbackBuffID", "0",        "custom"},//77
        {"Part2_SelfFeedbackCreateObjID","0",    "custom"},//78
        
        {"Part2_MultSkillTime",     "1",         "custom"},//79
        //SKPart3段-----------------------------------------------------------------
        {"Part3_SwitchType",        "0",         "default_SwitchType"},//80
        {"Part3_DetectType",        "1",         "default_DetectType"},//81
        {"Part3_TargetType",        "1",         "default_TargetType"},//82
		{"Part3_BeMultTarget",      "false",     "custom"},//83

        {"Part3_AnimResID",         "1",         "artRes_AnimResID"},//84
        {"Part3_AnimName",          "AnimName",  "custom"},//85
        {"Part3_AnimSpeedScale",    "1",         "custom"},//86

        {"Part3_AttackEffID",       "1",         "custom"},//87
        {"Part3_AnimEvent4AttackEff","1",        "custom"},//88

        {"Part3_MotionType",        "1",         "default_MotionType"},//89
        {"Part3_MotionSpeed",       "1",         "custom"},//90
        {"Part3_MotionMaxDis",      "1",         "custom"},//91

        {"Part3_CollideType",       "1",         "default_CollideType"},//92
        {"Part3_CollideRectW",      "1",         "custom"},//93
        {"Part3_CollideRectH",      "1",         "custom"},//94
        {"Part3_CollideCircleR",    "1",         "custom"},//95
        {"Part3_CollideSectorAngle","1",         "custom"},//96
        {"Part3_AnimEvent4Collide", "1",         "custom"},//97

        {"Part3_CreateObjID",       "0",         "custom"},//98
        {"Part3_AnimEvent4CreateObj","1",        "custom"},//99

        {"Part3_BuffID",            "0",         "custom"},//100
        {"Part3_AnimEvent4Buff",    "1",         "custom"},//101

        {"Part3_EnemyHurtType",     "1",         "default_EnemyHurtType"},//102
        {"Part3_EnemyHurtValue",    "1",         "custom"},//103
        {"Part3_EnemyHurtEffID",    "0",         "custom"},//104
        {"Part3_EnemyHurtState",    "1",         "default_EnemyHurtState"},//105
        {"Part3_EnemyHurtBuffID",   "0",         "custom"},//106
        {"Part3_EnemyHurtCreateObjID","0",       "custom"},//107
        {"Part3_EnemyHurtLinkType", "0",         "custom"},//108

        {"Part3_SelfFeedbackType",   "0",        "custom"},//109
        {"Part3_SelfFeedbackEffID",  "0",        "custom"},//110
        {"Part3_SelfFeedbackState",  "0",        "default_SelfFeedbackState"},//111
        {"Part3_SelfFeedbackBuffID", "0",        "custom"},//112
        {"Part3_SelfFeedbackCreateObjID","0",    "custom"},//113

        {"Part3_MultSkillTime",     "1",         "custom"},//114
        //技能升级数据段-----------------------------------技能升级最多10个等级，每级最多3个属性更改
        
		//技能等级内容介绍段---------------------------------
		{"level1_content",        "L1_content",  "custom"},//115
		{"level2_content",        "L2_content",  "custom"},//116
		{"level3_content",        "L3_content",  "custom"},//117
		{"level4_content",        "L4_content",  "custom"},//118
		{"level5_content",        "L5_content",  "custom"},//119
		{"level6_content",        "L6_content",  "custom"},//120
		{"level7_content",        "L7_content",  "custom"},//121
		{"level8_content",        "L8_content",  "custom"},//122
		{"level9_content",        "L9_content",  "custom"},//123
		{"level10_content",       "L10_content",  "custom"},//124

		//技能等级数据段--------------------------------------
        {"level1_attr1_type",       "1",         "default_level_attr_type"},//125
        {"level1_attr1_skpart",     "1",         "custom"},
        {"level1_attr1_value",      "1",         "custom"},
        {"level1_attr2_type",       "1",         "default_level_attr_type"},
        {"level1_attr2_skpart",     "1",         "custom"},
        {"level1_attr2_value",      "1",          "custom"},
        {"level1_attr3_type",       "1",         "default_level_attr_type"},
        {"level1_attr3_skpart",     "1",         "custom"},
        {"level1_attr3_value",      "1",         "custom"},

        {"level2_attr1_type",       "1",         "default_level_attr_type"},
        {"level2_attr1_skpart",     "1",         "custom"},
        {"level2_attr1_value",      "1",         "custom"},
        {"level2_attr2_type",       "1",         "default_level_attr_type"},
        {"level2_attr2_skpart",     "1",         "custom"},
        {"level2_attr2_value",      "1",         "custom"},
        {"level2_attr3_type",       "1",         "default_level_attr_type"},
        {"level2_attr3_skpart",     "1",         "custom"},
        {"level2_attr3_value",      "1",         "custom"},

        {"level3_attr1_type",       "1",         "default_level_attr_type"},
        {"level3_attr1_skpart",     "1",         "custom"},
        {"level3_attr1_value",      "1",         "custom"},
        {"level3_attr2_type",       "1",         "default_level_attr_type"},
        {"level3_attr2_skpart",     "1",         "custom"},
        {"level3_attr2_value",      "1",         "custom"},
        {"level3_attr3_type",       "1",         "default_level_attr_type"},
        {"level3_attr3_skpart",     "1",         "custom"},
        {"level3_attr3_value",      "1",         "custom"},

        {"level4_attr1_type",       "1",         "default_level_attr_type"},
        {"level4_attr1_skpart",     "1",         "custom"},
        {"level4_attr1_value",      "1",         "custom"},
        {"level4_attr2_type",       "1",         "default_level_attr_type"},
        {"level4_attr2_skpart",     "1",         "custom"},
        {"level4_attr2_value",      "1",         "custom"},
        {"level4_attr3_type",       "1",         "default_level_attr_type"},
        {"level4_attr3_skpart",     "1",         "custom"},
        {"level4_attr3_value",      "1",         "custom"},

        {"level5_attr1_type",       "1",         "default_level_attr_type"},
        {"level5_attr1_skpart",     "1",         "custom"},
        {"level5_attr1_value",      "1",         "custom"},
        {"level5_attr2_type",       "1",         "default_level_attr_type"},
        {"level5_attr2_skpart",     "1",         "custom"},
        {"level5_attr2_value",      "1",         "custom"},
        {"level5_attr3_type",       "1",         "default_level_attr_type"},
        {"level5_attr3_skpart",     "1",         "custom"},
        {"level5_attr3_value",      "1",         "custom"},

        {"level6_attr1_type",       "1",         "default_level_attr_type"},
        {"level6_attr1_skpart",     "1",         "custom"},
        {"level6_attr1_value",      "1",         "custom"},
        {"level6_attr2_type",       "1",         "default_level_attr_type"},
        {"level6_attr2_skpart",     "1",         "custom"},
        {"level6_attr2_value",      "1",         "custom"},
        {"level6_attr3_type",       "1",         "default_level_attr_type"},
        {"level6_attr3_skpart",     "1",         "custom"},
        {"level6_attr3_value",      "1",         "custom"},

        {"level7_attr1_type",       "1",         "default_level_attr_type"},
        {"level7_attr1_skpart",     "1",         "custom"},
        {"level7_attr1_value",      "1",         "custom"},
        {"level7_attr2_type",       "1",         "default_level_attr_type"},
        {"level7_attr2_skpart",     "1",         "custom"},
        {"level7_attr2_value",      "1",         "custom"},
        {"level7_attr3_type",       "1",         "default_level_attr_type"},
        {"level7_attr3_skpart",     "1",         "custom"},
        {"level7_attr3_value",      "1",         "custom"},

        {"level8_attr1_type",       "1",         "default_level_attr_type"},
        {"level8_attr1_skpart",     "1",         "custom"},
        {"level8_attr1_value",      "1",         "custom"},
        {"level8_attr2_type",       "1",         "default_level_attr_type"},
        {"level8_attr2_skpart",     "1",         "custom"},
        {"level8_attr2_value",      "1",         "custom"},
        {"level8_attr3_type",       "1",         "default_level_attr_type"},
        {"level8_attr3_skpart",     "1",         "custom"},
        {"level8_attr3_value",      "1",         "custom"},

        {"level9_attr1_type",       "1",         "default_level_attr_type"},
        {"level9_attr1_skpart",     "1",         "custom"},
        {"level9_attr1_value",      "1",         "custom"},
        {"level9_attr2_type",       "1",         "default_level_attr_type"},
        {"level9_attr2_skpart",     "1",         "custom"},
        {"level9_attr2_value",      "1",         "custom"},
        {"level9_attr3_type",       "1",         "default_level_attr_type"},
        {"level9_attr3_skpart",     "1",         "custom"},
        {"level9_attr3_value",      "1",         "custom"},

        {"level10_attr1_type",      "1",         "default_level_attr_type"},
        {"level10_attr1_skpart",    "1",         "custom"},
        {"level10_attr1_value",     "1",         "custom"},
        {"level10_attr2_type",      "1",         "default_level_attr_type"},
        {"level10_attr2_skpart",    "1",         "custom"},
        {"level10_attr2_value",     "1",         "custom"},
        {"level10_attr3_type",      "1",         "default_level_attr_type"},
        {"level10_attr3_skpart",    "1",         "custom"},
        {"level10_attr3_value",     "1",         "custom"},//214

		//{"end",						"end",		 "custom"} //215一条数据结束标识
    };

    #endregion

    #region 模板中关键数据的Index
    /// <summary>
    /// 记录下SKPartNum所在位置
    /// 这里是从0开始记录的第7项，就是第8个
    /// </summary>
    public const int INDEX_SKPartNum = 8;

	/// <summary>
	/// 一个skpart的数据条数
	/// </summary>
	public const int SKPart_SIZE = 35;

    /// <summary>
    /// SKPart2数据开始结束位置
    /// </summary>
    public const int INDEX_PART2_START = 45;
	public const int INDEX_PART2_END = INDEX_PART2_START+SKPart_SIZE-1;

    /// <summary>
    /// SKPart3数据开始结束位置
    /// </summary>
	public const int INDEX_PART3_START = INDEX_PART2_END+1;
	public const int INDEX_PART3_END = INDEX_PART3_START+SKPart_SIZE-1;

    /// <summary>
    /// 技能最大等级位置
    /// </summary>
    public const int INDEX_SKLEVLE_NUM = 9;

    /// <summary>
    /// 技能介绍开始结束位置
    /// </summary>
    public const int INDEX_LEVEL_CONTENT_START = 115;
	public const int INDEX_LEVEL_CONTENT_END = INDEX_LEVEL_CONTENT_START+10-1;//10就是技能最大等级数

    /// <summary>
    /// 等级信息开始位置
    /// </summary>
	public const int INDEX_LEVEL_START = INDEX_LEVEL_CONTENT_END+1;

    /// <summary>
    /// 技能最大有效数据，213之后都是无效数据
    /// </summary>
    public const int INDEX_SK_MAX_EFFECTIVE_DATA = 214;//645

	/// <summary>
	/// 主动技能模板在sql一行中存储的最大数据条数
	/// 这里实际有效数据条数是INDEX_SK_MAX_EFFECTIVE_DATA中的数据条数
	/// 多出来的条数，根据不同技能自己的情况自行扩展key，value
	/// </summary>
	/// <returns>The max sql data number.</returns>
    public static int GetMaxSqlDataNum() {
        return 660;
    }

    #endregion

    /// <summary>
    /// 一个KVData就是上面的一行数据
    /// KVDic就是存储了上面全部的数据
    /// </summary>
    /// <returns>The template data.</returns>
    public static Dictionary<int, KVData> GetTemplateData()
    {
        Dictionary<int, KVData> KVDic = new Dictionary<int, KVData>();

        int count = Content.GetLength(0);
        for (int i = 0; i < count; i++)
        {
            KVData data = new KVData();
            data.key = Content[i, 0];
            data.value = Content[i, 1];
            data.type = Content[i, 2];
            KVDic.Add(i, data);
        }


        //NINFO 补全数据由SQLitHelper4DataEditor.InsertValues来处理了，不需要各个模板自己处理
        //模板数据比sql中最大数据少多少
        //		int nullData =SQLiteHelper4DataEditor.MAX_NUM - count;
        //
        //		if (nullData>0){
        //			if ((nullData%3)!=0) {//因为每条数据都是3项，所以这个数一定是3的倍数，如果不是，说明数据错误，报警
        //				Log.e("SkillDataTemplate.GetTemplateData->为模板数据补全sql占位数据时发生错误 nulData="+nullData);
        //				return null;
        //			}
        //
        //			for(int i=0;i<nullData;i++){
        //				KVData data = new KVData();
        //				data.key = "";
        //				data.value = "";
        //				data.type = "";//这里应该指向一个能打开默认dialog的类型，""空串就指定到默认值
        //
        //				int tempInt = count + i;
        //				KVDic.Add (tempInt, data);
        //			}
        //		}

        return KVDic;

    }

    #region 主动技能冗余数据过滤
    /// <summary>
    /// 过滤(主动)技能数据
    /// 技能数据在dataEditor中包含冗余数据，所以要进行过滤
    /// 简单描述流程
    /// 1遍历所有treeItem
    /// 2遍历每一个treeItem数据项
    /// 3找到每个treeItem中的最大skpart项，最大level
    /// 4忽略掉冗余的skpart项，无用level数据，并把有用的数据直接存到buffer中
    /// 5最后把buffer转为byte[] 返回
    /// </summary>
    /// <param name="orignalData"></param>
    /// <returns></returns>
    public static byte[] FilterExportData(Dictionary<string, Dictionary<int, KVData>> orignalData)
    {
        IoBuffer _buffer = new IoBuffer(1000000);

		int dataNum = orignalData.Count;//技能条数

		_buffer.PutInt(dataNum);

        foreach (KeyValuePair<string, Dictionary<int, KVData>> p0 in orignalData)
        {  //遍历整个treeItem数据
            //Debug.Log("TDataExport=========================>ItemID:"+ p0.Key);
            Dictionary<int, KVData> _tempOrignalDic = p0.Value;                     //获取单个treeItme的全部数据项(KVData)
            string skillpartNumStr = _tempOrignalDic[INDEX_SKPartNum].value;       //获取skpartNum
            int skillpartNum = Convert.ToInt32(skillpartNumStr);
            string skillLevelNumStr = _tempOrignalDic[INDEX_SKLEVLE_NUM].value;     //获取sklevelNum
            int skillLevelNum = Convert.ToInt32(skillLevelNumStr);

            foreach (KeyValuePair<int, KVData> p1 in _tempOrignalDic)
            {   
                //遍历单个treeItem数据           
                //Debug.Log("TDataExport----------->ItemIndex:" + p1.Key);

                //过滤技能段冗余数据
                if (skillpartNum < 3)
                {
                    //过滤skpart3
                    //skpart3范围
                    //Debug.Log("--->bp3  key："+ p1.Key);
                    if (p1.Key >= INDEX_PART3_START && p1.Key <= INDEX_PART3_END)
                    {
                        //Debug.Log("--->p3");
                        continue;
                    }

                    if (skillpartNum < 2)
                    {
                        if (p1.Key >= INDEX_PART2_START && p1.Key <= INDEX_PART2_END)
                        {
                            //Debug.Log("--->p2");
                            continue;
                        }
                    }
                }

                //过滤冗余技能介绍
                if (p1.Key >= (INDEX_LEVEL_CONTENT_START + skillLevelNum) && p1.Key <= INDEX_LEVEL_CONTENT_END){
                    continue;
                }
              

                //过滤技能等级冗余数据
                if (skillLevelNum < 10)
                {
                    if (FilterSkLevelData(p1.Key, 10)) { /*Debug.Log("--->L10"); */; continue; }
                    if (skillLevelNum < 9)
                    {
                        if (FilterSkLevelData(p1.Key, 9)) { /*Debug.Log("--->L9"); */; continue; }
                        if (skillLevelNum < 8)
                        {
                            if (FilterSkLevelData(p1.Key, 8)) { /*.Log("--->L8"); */; continue; }
                            if (skillLevelNum < 7)
                            {
                                if (FilterSkLevelData(p1.Key, 7)) { /*.Log("--->L7"); */; continue; }
                                if (skillLevelNum < 6)
                                {
                                    if (FilterSkLevelData(p1.Key, 6)) { /*.Log("--->L6"); */; continue; }
                                    if (skillLevelNum < 5)
                                    {
                                        if (FilterSkLevelData(p1.Key, 5)) { /*.Log("--->L5"); */continue; }
                                        if (skillLevelNum < 4)
                                        {
                                            if (FilterSkLevelData(p1.Key, 4)) { /*.Log("--->L4"); */ continue; }
                                            if (skillLevelNum < 3)
                                            {
                                                if (FilterSkLevelData(p1.Key, 3)) { /*.Log("--->L3"); */ continue; }
                                                if (skillLevelNum < 2)
                                                {
                                                    if (FilterSkLevelData(p1.Key, 2)) { /*.Log("--->L2"); */ continue; }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }//if <10结束

                //过滤掉尾部冗余数据
                if (p1.Key > INDEX_SK_MAX_EFFECTIVE_DATA)
                {
                    //Debug.Log("--->143");
                    continue;
                }

                //Debug.Log("TExportData------->key:" + p1.Value.key);
                //NINFO 没被过滤的就直接写入二进制缓存
                if(GEditorDataMgr.BeTest)_buffer.PutString(p1.Value.key);
                _buffer.PutString(p1.Value.value);
                if(GEditorDataMgr.BeTest) _buffer.PutString(p1.Value.type);

            }//遍历单个treeItem数据 foreach结束

        }//遍历整个treeItem数据 foreach结束

        byte[] bs = _buffer.ToArray();
        return bs;
    }

    /// <summary>
    /// 判定技能等级是否过滤
    /// 具体就是判断curIndex是否在skillLevelNum所包含的index范围内
    /// 返回true代表过滤
    /// false代表不过滤
    /// </summary>
    /// <param name="curIndex"></param>
    /// <param name="skillLevelNum">代表第几级的数据段，比如等级1就是114-116</param>
    /// <returns></returns>
    private static bool FilterSkLevelData(int curIndex, int skillLevelNum)
    {

        int start = GetLevelDataStartIndex(skillLevelNum);
        int end = start + 8;

        //Debug.Log("L10Fuck--->start:" + start + " end:" + end + " curIndex:" + curIndex);

        if (curIndex >= start && curIndex <= end)
        {
            return true;
        }
        return false;
    }

    /// <summary>
    /// 查找某一技能等级数据(在技能数据模板中)开始位置
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    private static int GetLevelDataStartIndex(int level)
    {
        int index = INDEX_LEVEL_START + (level - 1) * 9;
        return index;
    }

    /// <summary>
    /// 查找某一技能等级数据(在技能数据模板中)结束位置
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    //private static int GetLevelDataEndIndex(int level)
    //{
    //    int index = GetLevelDataStartIndex(level) + 2;
    //    return index;
    //}
    #endregion

    #region 主动技能数据模板中特殊数据项的dropdown选项处理

	//default_SKOperateType

	/// <summary>
	/// SK释放时的操作方式
	/// </summary>
	public static string[,] ContentDropdownData_SKOperateType = {
		{ "0（瞬发）"           		,"0"},
		{ "1（方向性技能）"           ,"1"},
		{ "2（目标性技能）"           ,"2"},
	};

    /// <summary>
    /// SKPart可用切换方式显示名，及对应数值信息
    /// 对应到模板数据是
    /// Part1_SwitchType,Part2_SwitchType,Part3_SwitchType
    /// </summary>
    public static string[,] ContentDropdownData_SwitchType = {
        { "0（顺序执行）"           ,"0"},
        { "1（并行执行）"           ,"1"},
        { "2（延时执行）"           ,"2"},
    };

    /// <summary>
    /// SKPart技能检测方式
    /// </summary>
    public static string[,] ContentDropdownData_DetectType = {
        { "0（锁定目标）"                ,"0"},
        { "1（先检测碰撞后执行技能）"    ,"1"},
        { "2（无目标执行后检测碰撞）"    ,"2"},
    };

    /// <summary>
    /// 技能作用目标类型
    /// 用byte的低3位分别表示敌方，友方，自己，用或的方式组合使用
    /// 01代表是敌方
    /// 10代表是友方
    /// 100代表是自己
    /// </summary>
    public static string[,] ContentDropdownData_TargetType = {
        { "1（对敌方生效）"                ,"1"},
        { "2（对友军生效）"                ,"2"},
        { "3（对双方生效）"                ,"3"},
        { "4（对自己生效）"                ,"4"},
        { "6（对自己和友军生效）"          ,"6"}
    };

    public static string[,] ContentDropdownData_MotionType = {
        { "0（无运动）"               ,"0"},
        { "1（直线）"                 ,"1"},
        { "2 (抛物线)"                ,"2"},
        { "3（瞬移）"                 ,"3"},
    };

    public static string[,] ContentDropdownData_CollideType = {
        { "0（无碰撞）"               ,"0"},
        { "1（矩形）"                 ,"1"},
        { "2 (圆扇形)"                ,"2"},
    };

    public static string[,] ContentDropdownData_EnemyHurtType = {
        { "1（物理）"                 ,"1"},
        { "2 (法术)"                  ,"2"},
        { "3 (其他)"                  ,"3"},
    };

    public static string[,] ContentDropdownData_EnemyHurtState = {
        { "0（无效果）"               ,"0"},
        { "1（后仰）"                 ,"1"},
        { "2 (击飞)"                  ,"2"},
        { "3 (击倒)"                  ,"3"},
        { "4 (击退)"                  ,"4"},
    };

    public static string[,] ContentDropdownData_SelfFeedbackState = {
        { "0（无效果）"               ,"0"},
        { "1（后仰）"                 ,"1"},
        { "2 (击飞)"                  ,"2"},
        { "3 (击倒)"                  ,"3"},
        { "4 (击退)"                  ,"4"},
    };

    public static string[,] ContentDropdownData_level_attr_type = {
        { "1（血）"                   ,"1"},
        { "2 (蓝)"                    ,"2"},
        { "3 (冷却)"                  ,"3"},
        { "4 (距离)"                  ,"4"},
        { "5 (其他未想到的)"          ,"5"},
    };

    
    /// <summary>
    /// 根据(dataType)获取(主动技能)contentDropdown数据
    /// </summary>
    /// <param name="dataType"></param>
    /// <returns></returns>
    public static Dictionary<string, string> GetContentDropdownDataList(string dataType){

        Dictionary<string, string> dic = null;

        Log.i("ActiveSkillDataTemplate", "GetContentDropdownDataList","dataType:"+dataType);

        switch (dataType)
        {
			case "default_SKOperateType":
			dic = ProcessDefaultDropdownData(ContentDropdownData_SKOperateType);
				break;
            case "default_SwitchType":
                dic = ProcessDefaultDropdownData(ContentDropdownData_SwitchType);
                break;
            case "default_DetectType":
                dic = ProcessDefaultDropdownData(ContentDropdownData_DetectType);
                break;
            case "default_TargetType":
                dic = ProcessDefaultDropdownData(ContentDropdownData_TargetType);
                break;
            case "artRes_AnimResID":
                dic = ProcessArtResDropdownData(GEditorEnum.EDITOR_ART_RES_ROOTURL + "role/");
                break;
            case "default_MotionType":
                dic = ProcessDefaultDropdownData(ContentDropdownData_MotionType);
                break;
            case "default_CollideType":
                dic = ProcessDefaultDropdownData(ContentDropdownData_CollideType);
                break;
            case "default_EnemyHurtType":
                dic = ProcessDefaultDropdownData(ContentDropdownData_EnemyHurtType);
                break;
            case "default_EnemyHurtState":
                dic = ProcessDefaultDropdownData(ContentDropdownData_EnemyHurtState);
                break;
            case "default_SelfFeedbackState":
                dic = ProcessDefaultDropdownData(ContentDropdownData_SelfFeedbackState);
                break;
            case "default_level_attr_type":
                dic = ProcessDefaultDropdownData(ContentDropdownData_level_attr_type);
                break;
            default:
                Log.e("ActiveSkillDataTemplate", "GetContentDropdownDataList", "未知的数据类型type：" + dataType,BeShowLog);
                break;
        }

        //Log.i("ActiveSkillDataTemplate", "GetContentDropdownDataList", "dic.len:" + dic.Count);

        return dic;
    }

    /// <summary>
    /// 获取来自其模板类的固定数据，并生成dropdown数据
    /// </summary>
    /// <param name="data">模板类的固定数据</param>
    /// <returns></returns>
    private static Dictionary<string, string> ProcessDefaultDropdownData(string[,] data)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();
        for (int i = 0; i < data.GetLength(0); i++){
            dic.Add(data[i, 0], data[i, 1]);
        }
        return dic;
    }

    /// <summary>
    /// 获取来自其他编辑器引用数据，并生成dropdown数据
    /// </summary>
    /// <param name="editorType">引用的是哪个editor的数据</param>
    /// <returns></returns>
    private static Dictionary<string, string> ProcessEditorRefDropdownData(string editorType)
    {
        Dictionary<string, string> dic = new Dictionary<string, string>();

        TreeItemData[] _data = GEditorRoot.GetIns().TreeContainerDic[editorType].GetAllTreeItemData();
        for (int i = 0; i < _data.Length; i++)
        {
            dic.Add(_data[i].TreeItemName, _data[i].TreeItemID);
        }

        return dic;
    }

    /// <summary>
    /// 获取来自某个资源文件夹下全部文件路径，并生成dropdown数据
    /// </summary>
    /// <param name="artResFolderURL"></param>
    /// <returns></returns>
    private static Dictionary<string, string> ProcessArtResDropdownData(string artResFolderURL)
    {
		//NINFO 数据编辑器中，资源的位置位于工程目录(跟Asset目录统计)res目录下
		//工程目录 C:/root/work/svnWorkspace/DateEditor/trunk
		string projectPath = Path.GetDirectoryName (Application.dataPath);

		//C:/root/work/svnWorkspace/DateEditor/trunk/
		string spath = projectPath + "/";

        Dictionary<string, string> dic = new Dictionary<string, string>();

		//获取当前目录artResFolderURL下所有文件的绝对路径
        string[] urls = Directory.GetFiles(artResFolderURL);

        for (int i = 0; i < urls.Length; i++)
        {
			//这里显示名，实际值都用资源的相对地址，现在要算出资源相对地址
            //string fileName = Path.GetFileName(urls[i]);

			//举例说明，这里就是用
			//美术资源文件绝对地址和  C:/root/work/svnWorkspace/DateEditor/trunk/res/role/hx_anim_sk1.n
			//资源文件夹根路径相减    C:/root/work/svnWorkspace/DateEditor/trunk/
			//得到一个美术文件的相对地址  res/role/hx_anim_sk1.n 注意res前面没/,这个时历史原因导致
			string relativePath = urls [i].Substring(spath.Length);
			//Debug.Log ("===========>relePath:"+relativePath);
            //NINFO 注意这里的值直接使用文件名，不使用完整路径或相对路径，game解析时，一定知道文件类型，所以能找到文件所在位置，所以只要文件名称就够了
			dic.Add(relativePath, relativePath /*urls[i]*/);//显示名和引用名称分别是资源文件名(带后缀)，资源相对路径

        }
        return dic;
    }

    #endregion

}
    

