using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using UnityEngine;

public class Director : MonoBehaviour
{
    public static Director Inst { get; private set; }
    private void Awake()
    {
        if (Inst == null) Inst = this; else Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }

    private ITransitionEffect TEffect;
    private string currentTEffectName = "Anim2";

    public TransitionImage Background; //�I��
    public float BgTransitionDur = 2f;
    private Dictionary<string, string> imagePaths = new Dictionary<string, string>(); // �x�s�Ϥ��귽���|

    // �w�q��Ʀr��
    private Dictionary<string, Action<object[]>> actions;
    private void Start()
    {
        // ���oTEffect�ե�
        TEffect = GetComponent<TEffectsManager>().Init(currentTEffectName);
        // ��l�Ʀr��øj�w���
        actions = new Dictionary<string, Action<object[]>>
        {
            { "��", args => ChangeExpression(args) },
            { "����", args => MoveCharacter(args) },
            { "�H����", args => SetRandomValue(args) },
            { "����", args => SetMoney(args) },
            { "�H�J", args => FadeIn(args) },
            { "�H�X", args => FadeOut(args) },
            { "����", args => Cut(args) },
            { "�I��", args => SetBackground(args) },
        };

        // ��l�ƹϤ��귽���|
        imagePaths["��D"] = "Resources://Sprites/AVG/BG/Landscape/Daily/AChos001_19201080.jpg";
        imagePaths["����"] = "Resources://Sprites/AVG/BG/Landscape/Daily/130machi_19201080.jpg";

        // ����
        //ExecuteAction("����,����1,10,20");
        //ExecuteAction("����,100");
        //ExecuteAction("����+100-20");
        //ExecuteAction("���� = 10");
        //ExecuteAction("�m�W = \"\"��i�Ҵ���\"\"");
        //ExecuteAction("�}�� = ��Ӱ}��");
        //ExecuteAction("HP=MaxHP*50");
        //ExecuteAction("��,�i�N�M,��");
    }
    public void Off()
    {
        TEffect.Stop();
        Background.Clear();
    }

    public void FadeIn()
    {
        StartCoroutine(FadeInWithDelay());
    }

    public void FadeOut()
    {
        StartCoroutine(FadeOutWithDelay());
    }

    public IEnumerator FadeInWithDelay(float Delay = 0f)
    {
        yield return new WaitForSeconds(Delay);
        TEffect.FadeIn();
    }

    public IEnumerator FadeOutWithDelay(float Delay = 0f)
    {
        yield return new WaitForSeconds(Delay);
        TEffect.FadeOut();
    }

    // �r�夤�����

    private void SetBackground(object[] args = null)
    {
        // �ˬd args �O�_�� null �ΪżƲ�
        if (args == null || args.Length == 0)
        {
            Debug.LogError("args is null or empty.");
            return;
        }

        // ���� imagePath
        string imagePath = null;
        if (args.Length > 0 && args[0] != null)
        {
            var key = args[0].ToString();
            if (!imagePaths.ContainsKey(key))
            {
                Debug.LogError($"Key '{key}' does not exist in imagePaths.");
                return;
            }
            imagePath = imagePaths[key];
        }
        else
        {
            Debug.LogError("args[0] is null or missing.");
            return;
        }

        // ���� effectType
        string effectType = null;
        if (args.Length > 1)
        {
            effectType = args[1]?.ToString(); // �p�G args[1] �� null�AeffectType �]�� null
            //Debug.Log($"effectType���ȡG{effectType}");
        }

        // ���� duration�A�q�{�� 0
        float duration = BgTransitionDur;
        if (args.Length > 2)
        {
            if (!float.TryParse(args[2]?.ToString(), out duration))
            {
                Debug.LogWarning($"Invalid duration value: {args[2]}. Using default duration: {duration}.");
            }
        }
        Background.StartTransition(imagePath, effectType, duration);
    }

    private void Cut(object[] args = null)
    {
        AVG.Inst.nextCutIndex = 99999; //�ϤU�@�d���޶W�X�d��A�j���
    }

    private void FadeIn(object[] args = null)
    {
        TEffect.FadeIn();
    }

    private void FadeOut(object[] args = null)
    {
        TEffect.FadeOut();
    }

    private void SetMoney(object[] args = null)
    {
        string input = args[0].ToString();
        PPM.Inst.Set("����", input);
    }

    /// <summary>
    /// �]�w�H���ȡC
    /// </summary>
    /// <param name="args">�Ѽư}�C�Aargs[0] ���H���]�w�r��</param>
    private void SetRandomValue(object[] args)
    {
        if (args == null || args.Length == 0 || args[0] == null)
        {
            Debug.LogError("�Ѽƿ��~�Aargs[0] ����");
            return;
        }

        // �N args[0] �ର�r��
        string input = args[0].ToString();

        if (input.Contains("~"))
        {
            // �B�z "~" �H���d��
            string[] parts = input.Split('~');

            if (parts.Length == 2)
            {
                string startStr = parts[0].Trim();
                string endStr = parts[1].Trim();

                if (int.TryParse(startStr, out int startInt) && int.TryParse(endStr, out int endInt))
                {
                    // ��ƽd��
                    int randomInt = UnityEngine.Random.Range(startInt, endInt + 1);
                    PPM.Inst.Set("�H����", randomInt.ToString());
                }
                else if (float.TryParse(startStr, out float startFloat) && float.TryParse(endStr, out float endFloat))
                {
                    // �B�I�ƽd��
                    float randomFloat = UnityEngine.Random.Range(startFloat, endFloat);

                    // ���o�p�Ʀ��
                    int decimalPlaces = Math.Max(startStr.Length - startStr.IndexOf('.') - 1, endStr.Length - endStr.IndexOf('.') - 1);

                    // �榡�Ƭ��ۦP�p�Ʀ��
                    string formattedFloat = randomFloat.ToString("F" + decimalPlaces);
                    PPM.Inst.Set("�H����", formattedFloat);
                }
                else
                {
                    Debug.LogError("�H���ȽШD���e��Ȥ��O�X�k�ƭ�" + input);
                }
            }
            else
            {
                Debug.LogError("�H���ȽШD�榡���~: " + input);
            }
        }
        else if (input.Contains("&"))
        {
            // �B�z "&" �H�����
            string[] options = input.Split('&');

            if (options.Length > 0)
            {
                string randomOption = options[UnityEngine.Random.Range(0, options.Length)].Trim();
                PPM.Inst.Set("�H����", randomOption);
            }
            else
            {
                Debug.LogError("�H���ȽШD�ʤֿﶵ: " + input);
            }
        }
        else
        {
            Debug.LogError("�H���ȽШD�Ѽƿ��~: " + input);
        }
    }

    private void ChangeExpression(object[] args)
    {
        string character = args[0]?.ToString();
        string expression = args[1]?.ToString();
        Debug.Log($"Change {character}'s expression to {expression}");
    }

    private void MoveCharacter(object[] args)
    {
        string character = args[0]?.ToString();
        float x = float.Parse(args[1].ToString());
        float y = float.Parse(args[2].ToString());
        Debug.Log($"Move {character} to position ({x}, {y})");
    }

    // ������

    public void ExecuteActionPackage(string[] commands)
    {
        if (commands == null || commands.Length == 0)
        {
            Debug.LogWarning("No commands provided for execution.");
            return;
        }

        foreach (var command in commands)
        {
            if (!string.IsNullOrWhiteSpace(command))
            {
                //Debug.Log($"[Pkg����]: {command}");
                ExecuteAction(command); // �եγ�ӫ��O��������
            }
            else
            {
                Debug.LogWarning("Skipped empty or null command.");
            }
        }
    }

    public void ExecuteAction(string command)
    {
        command = PreprocessCommand(command);// �ѪRcommand�í���
        Debug.Log($"����ѪR��cmd:{command}");

        var parts = command.Split(',').Select(part => part.Trim()).ToArray(); // Trim�B�z

        if (parts.Length >= 1 && actions.ContainsKey(parts[0]))
        {
            // �����ʧ@�W�٩M�Ѽ�
            string actionName = parts[0];
            object[] parameters = parts[1..]; // �����ѤU���Ѽ�

            // ������������
            actions[actionName](parameters);
        }
        else
        {
            Debug.LogWarning("Invalid command or action not found.");
        }
    }

    public string PreprocessCommand(string command)
    {
        //Debug.Log($"�}�l�B�z���O�G{command}");

        // 1. �P�_�O�_���B�⦡�B���]�t "," �P "="
        if (!command.Contains(",") && !command.Contains("=") && IsExpression(command))
        {
            // ���Ĥ@�ӹB��Ū���m
            int operatorIndex = command.IndexOfAny(new[] { '+', '-', '*', '/' });
            if (operatorIndex > 0)
            {
                // �H�B��Ť��� command
                string actionNamePre = command.Substring(0, operatorIndex).Trim();
                string expressionPre = command.Substring(operatorIndex).Trim();

                // ���լ� "actionName,actionName+expression"
                command = $"{actionNamePre},{actionNamePre}{expressionPre}";
                //Debug.Log($"���չB�⦡���O: {command}");
            }
        }
        // 2. ���Ϋ��O
        var parts = command.Split(',').Select(part => part.Trim()).ToArray(); // Trim�B�z

        if (parts.Length == 0)
        {
            Debug.LogWarning("Command is empty.");
            return command; // ��^��l���O
        }

        string actionName;
        // 3. �B�z�Ĥ@�� part�]��ƦW�١^
        var firstPart = parts[0].Split('=').Select(part => part.Trim()).ToArray(); // Trim�B�z
        if (firstPart.Length > 1)
        {
            // �p�G�� "="�A�������e�������@����ƦW��
            actionName = firstPart[0];

            // �N�����᪺��������ô��J�� parts �}�C��
            List<string> newParts = new List<string> { actionName }; // �s�}�C���Ĥ@�����O��ƦW��
            newParts.AddRange(firstPart[1..]); // ���J�����᪺����
            newParts.AddRange(parts[1..]); // ���J�쥻 parts ���Ѿl����

            // �N newParts ��s�^ parts
            parts = newParts.ToArray();
        }
        else
        {
            // �p�G�S�� "="�A�Ĥ@�� part �����N�O��ƦW��
            actionName = parts[0];
        }
        //Debug.Log($"��ƦW�١G{actionName}");

        // 4. �B�z�Ѿl�� parts
        for (int i = 1; i < parts.Length; i++)
        {
            string part = parts[i];

            // �p�G�O�Q���޸��]�q�A�������Ȭ��¦r��
            if (part.StartsWith("\"") && part.EndsWith("\""))
            {
                parts[i] = part.Trim('\"');
                //Debug.Log($"{part}�Q���޸��]�q");
            }
            else
            {
                // �_�h���ձq PlayerPrefs ������
                string prefValue = PPM.Inst.Get(part, part); // �q�{��^ part ����
                //Debug.Log($"{part}�S���Q���޸��]�q�A���ձqPP���ȡG{prefValue}");
                parts[i] = prefValue;

                // �p�G�O�⦡�A���ոѪR�íp��
                if (IsExpression(prefValue))
                {
                    //Debug.Log($"{prefValue}�O�⦡");
                    try
                    {
                        float result = EvaluateExpression(prefValue);
                        parts[i] = result.ToString();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogError($"Failed to evaluate expression: {prefValue}, Error: {ex.Message}");
                    }
                }
            }
        }

        // 5. �����̲׫��O
        string resultCmd = string.Join(",", new[] { actionName }.Concat(parts[1..]));
        //Debug.Log($"�����̲׫��O�G{resultCmd}");
        return resultCmd;
    }

    private bool IsExpression(string input)
    {
        // ²��P�_�O�_�]�t�B���
        return input.Contains("+") || input.Contains("-") || input.Contains("*") || input.Contains("/");
    }

    private float EvaluateExpression(string expression)
    {
        // �䴩���B���
        char[] operators = { '+', '-', '*', '/' };

        // ���Ϊ�F���ëO�d�B���
        List<string> tokens = new List<string>();
        int lastIndex = 0;

        for (int i = 0; i < expression.Length; i++)
        {
            if (operators.Contains(expression[i]))
            {
                // �N�B��ƩM�B��Ť��O�[�J
                tokens.Add(expression.Substring(lastIndex, i - lastIndex).Trim());
                tokens.Add(expression[i].ToString());
                lastIndex = i + 1;
            }
        }

        // �̫�@�ӹB���
        if (lastIndex < expression.Length)
        {
            tokens.Add(expression.Substring(lastIndex).Trim());
        }

        // �B�z�C�� token
        for (int i = 0; i < tokens.Count; i++)
        {
            if (!operators.Contains(tokens[i][0]))
            {
                // �D�B��šA�����ܼƦW�٩μƭ�
                string token = tokens[i].Trim();

                // �q PlayerPrefs ���ը��ȡA�q�{�� token ����
                if (!PlayerPrefs.HasKey(token))
                {
                    Debug.LogWarning($"[EvaluateExpression]{token}�L��");
                }
                string value = PPM.Inst.Get(token, "0");

                // ���ձN�ȸѪR���B�I��
                if (float.TryParse(value, out float number))
                {
                    tokens[i] = number.ToString(); // �N�ѪR���G��s�^ token
                }
                else
                {
                    throw new FormatException($"Invalid value for token: {token}");
                }
            }
        }

        // ���ժ�F��
        string processedExpression = string.Join("", tokens);

        // �p���F�����G
        DataTable table = new DataTable();
        object result = table.Compute(processedExpression, string.Empty);
        return Convert.ToSingle(result);
    }
}
