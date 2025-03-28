using Synty.SidekickCharacters.Database;
using UnityEngine;

public static class DatabaseService
{
    private static DatabaseManager _instance;
    private static bool _hasTriedInit = false;
    private static bool _connectionIsValid = false;

    public static DatabaseManager Instance
    {
        get
        {
            if (!_hasTriedInit)
            {
                _hasTriedInit = true;
                _instance = new DatabaseManager();
                _connectionIsValid = _instance.GetCurrentDbConnection() != null;

                if (_connectionIsValid)
                    Debug.Log("✅ Database connection established.");
                else
                    Debug.LogError("❌ Database connection failed.");
            }

            if (!_connectionIsValid)
            {
                return null; // Important: don't return broken instance
            }

            return _instance;
        }
    }

    public static bool IsInitialized => _connectionIsValid;

    public static void Reset()
    {
        _instance = null;
        _hasTriedInit = false;
        _connectionIsValid = false;
    }
}
