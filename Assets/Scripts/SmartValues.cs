using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utility;

namespace SmartValues
{
    /// <summary>
    /// Контейнер для переменной, значение которой сохраняется в PlayerPrefs,
    /// имеет событие обновления переменной
    /// </summary>
    public class SavedIntValue
    {
        private readonly string _name;
        private readonly int _defaultValue;
        private int? _value;

        public Action<int> OnValueUpdate;

        public int Value
        {
            get
            {
                if (!_value.HasValue)
                {
                    _value = PlayerPrefs.GetInt(_name, _defaultValue);
                    PlayerPrefs.SetInt(_name, _value.Value);
                }
                return _value.Value;
            }
            set
            {
                _value = value;
                OnValueUpdate?.Invoke(value);
                PlayerPrefs.SetInt(_name, value);
            }
        }

        public SavedIntValue(string name, int defaultValue = 0)
        {
            _name = name;
            _defaultValue = defaultValue;
        }
    }

    public class SavedStringValue
    {
        private readonly string _name;
        private readonly string _defaultValue;
        private string _value;

        public Action<string> OnValueUpdate;

        public string Value
        {
            get
            {
                if (_value == null)
                {
                    _value = PlayerPrefs.GetString(_name, _defaultValue);
                    PlayerPrefs.SetString(_name, _value);
                }
                return _value;
            }
            set
            {
                _value = value;
                OnValueUpdate?.Invoke(value);
                PlayerPrefs.SetString(_name, value);
            }
        }

        public SavedStringValue(string name, string defaultValue = "")
        {
            _name = name;
            _defaultValue = defaultValue;
        }
    }

    /// <summary>
    /// Контейнер для целочисленной переменной, значение которой зависит от другой переменной за счёт маппера
    /// Имеет событие обновления переменной
    /// </summary>
    public class DependentIntValue
    {
        private readonly List<int> _mapper;

        private int? _value;

        public Action<int> OnValueUpdate;

        public int Value => _value.Value;

        private void ResolveDependency(int value)
        {
            for (int i = 0; i < _mapper.Count; i++)
            {
                if (value >= _mapper[i])
                {
                    _value = i;
                }
                else break;
            }
            OnValueUpdate?.Invoke(_value.Value);
        }

        public DependentIntValue(List<int> mapping, SavedIntValue dependence)
        {
            _mapper = mapping;
            dependence.OnValueUpdate += ResolveDependency;
            ResolveDependency(dependence.Value);
        }
    }

    public class DependentList<T>
    {
        private readonly Dictionary<T, int> _keyValuePairs;

        private List<T> _keys;

        public Action<List<T>> OnKeysUpdate;

        public List<T> Keys => _keys;

        private void ResolveDependency(int lastValue)
        {
            _keys = new List<T>();
            foreach (var item in _keyValuePairs)
            {
                if (item.Value <= lastValue)
                {
                    _keys.Add(item.Key);
                }
            }
            OnKeysUpdate?.Invoke(_keys);
        }
        public DependentList(Dictionary<T, int> keyValuePairs, ref Action<int> dependence)
        {
            _keyValuePairs = keyValuePairs;
            dependence += ResolveDependency;
        }
    }


    /// <summary>
    /// Контейнер для события, автоматически очищается при загрузке сцены
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CleanAction<T>
    {
        public event Action<T> OnAction;

        public CleanAction()
        {
            SceneManager.activeSceneChanged += Clear;
        }

        public void Invoke(T value)
        {
            OnAction?.Invoke(value);
        }

        private void Clear(Scene from, Scene to)
        {
            OnAction = null;
        }
    }

    /// <summary>
    /// Контейнер для события, автоматически очищается при загрузке сцены
    /// </summary>
    public class CleanAction
    {
        public event Action OnAction;

        public CleanAction()
        {
            SceneManager.activeSceneChanged += Clear;
        }

        public void Invoke()
        {
            OnAction?.Invoke();
        }

        private void Clear(Scene from, Scene to)
        {
            OnAction = null;
        }
    }

    /// <summary>
    /// Контейнер для объектов, автоматически очищается при загрузке сцены
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CleanValue<T> where T : new()
    {
        public T Value { get; set; }

        public CleanValue()
        {
            Value = new T();
            SceneManager.activeSceneChanged += Clear;
        }

        private void Clear(Scene from, Scene to)
        {
            Value = new T();
        }
    }

    public class BsonObject<T> where T : new()
    {
        private T _value;

        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();

        public T Value
        {
            get => _value;
            set => _value = value;
        }

        private readonly string _name;

        public void Save()
        {
            sw.Start();
            BsonUtility.ToFiles(_name, _value);
            sw.Stop();
            Debug.Log("Serialization of BSON " + _name +" lasted " + TimeSpan.FromTicks(sw.ElapsedTicks).TotalMilliseconds.ToString("F") + " miliseconds.");
            sw.Reset();
        }

        public BsonObject(string name)
        {
            _name = name;
            try
            {
                sw.Start();
                _value = BsonUtility.FromFiles<T>(_name);
                sw.Stop();
                Debug.Log("Deserialization of BSON " + _name + " lasted " + TimeSpan.FromTicks(sw.ElapsedTicks).TotalMilliseconds.ToString("F") + " miliseconds.");
                sw.Reset();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                _value = new T();
            }
            finally
            {
                if (_value == null)
                    _value = new T();
            }
        }
    }
}