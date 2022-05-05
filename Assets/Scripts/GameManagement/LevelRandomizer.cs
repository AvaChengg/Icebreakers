using UnityEngine;

public class LevelRandomizer : MonoBehaviour
{
    [SerializeField] private GameObject[] _levelPrefabs;
    private static int _lastLevelLoaded = 0;
    private static bool _firstLoad = true;

    public bool LoadRandomLevel()
    {
        if (_levelPrefabs.Length == 0) return false;

        if (_firstLoad)
        {
            int randomLevel = Random.Range(0, _levelPrefabs.Length);
            _levelPrefabs[randomLevel].SetActive(true);
            _lastLevelLoaded = randomLevel;
            _firstLoad = false;
        }
        else
        {
            _lastLevelLoaded++;
            if(_lastLevelLoaded >= _levelPrefabs.Length)
            {
                _lastLevelLoaded = 0;
            }

            _levelPrefabs[_lastLevelLoaded].SetActive(true);
        }

        return true;
    }
}
