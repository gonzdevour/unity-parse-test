#if UNITY_EDITOR
using UnityEditor; // ���� Unity �s�边�����\�઺�R�W�Ŷ�
using UnityEngine; // ���� Unity �����֤ߥ\�઺�R�W�Ŷ�
using UnityEngine.SceneManagement; // ���ѳ����޲z�����\�઺�R�W�Ŷ�

// �аO���b�s�边�Ҧ��U��l�ƪ��ݩ�
[InitializeOnLoad]
public class AutoGlobalSceneLoader
{
    // �`�ơA�Ω���w�����������W��
    private const string GlobalSceneName = "Global";

    // �R�A�c�y��ơA��s�边�[�������ɷ|�۰ʰ���
    static AutoGlobalSceneLoader()
    {
        // �q�\�s�边���C���Ҧ����A�ܧ�ƥ�
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
    }

    // ��C���Ҧ����A�o���ܤƮ�Ĳ�o���^�ը��
    private static void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        // �P�_�O�_�i�J�C���Ҧ�
        if (state == PlayModeStateChange.EnteredPlayMode)
        {
            // �ˬd���������O�_�w�[��
            if (!SceneManager.GetSceneByName(GlobalSceneName).isLoaded)
            {
                // �p�G���[���A��X��x�H��
                Debug.Log($"Auto-loading Global Scene: {GlobalSceneName}");

                // �[�����������A�ϥΪ��[�Ҧ��]�O�d��e�����^
                SceneManager.LoadScene(GlobalSceneName, LoadSceneMode.Additive);
            }
        }
    }
}
#endif
