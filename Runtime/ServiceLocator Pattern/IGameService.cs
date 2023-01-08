using UnityEngine;
using System;
using UnityEditor;
using System.ComponentModel;
using System.Threading.Tasks;

namespace com.eyerunnman.patterns
{
    /// <summary>
    /// Base interface for our service locator to work with. Services implementing
    /// this interface will be retrievable using the locator.
    /// </summary> 
    public interface IGameService
    {
        public void OnRegister();
        public void OnUnRegister();
        public void OnRegisterError(string errMsg);
        public Task Initialize(bool overrideService = false);

        public ServiceState ServiceState { get; }

    }
    [Serializable]
    public enum ServiceState
    {
        Undefined,
        Waiting,
        Registering,
        Registered,
        UnRegistered,
        RegisterError
    }

    public abstract class GameService : MonoBehaviour, IGameService
    {   
        private ServiceState _serviceState = ServiceState.Waiting;
        public ServiceState ServiceState => _serviceState;

        public void OnRegister()
        {
            _serviceState = ServiceState.Registered;
        }

        public void OnRegisterError(string errMsg)
        {
            _serviceState = ServiceState.RegisterError;

            throw new Exception(errMsg);
        }

        public void OnUnRegister()
        {
            _serviceState = ServiceState.UnRegistered;
        }

        public virtual async Task Initialize(bool overrideService)
        {
            _serviceState = ServiceState.Registering;
        }
    }

}