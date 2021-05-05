using System;
using System.Collections.Generic;
using System.Web;

namespace Products.Tests.Common.Fakes
{
    public class FakeHttpSession : HttpSessionStateBase
    {
        private readonly IDictionary<string, object> _sessionValues;

        private readonly Exception _exception;

        public FakeHttpSession()
        {
            _sessionValues = new Dictionary<string, object>();
        }

        // ReSharper disable once UnusedMember.Global
        public FakeHttpSession(IDictionary<string, object> sessionValues)
        {
            _sessionValues = sessionValues;
        }

        // ReSharper disable once UnusedMember.Global
        public FakeHttpSession(Exception exception)
        {
            _exception = exception;
        }

        public override object this[string key]
        {
            get
            {
                if (_exception != null)
                {
                    throw _exception;
                }

                if (key != null && _sessionValues.ContainsKey(key))
                {
                    return _sessionValues[key];
                }

                return null;
            }
            set => _sessionValues[key] = value;
        }

        public override void Add(string name, object value)
        {
            _sessionValues.Add(name, value);
        }

        public override int Count => _sessionValues.Count;

        public override void Abandon()
        {
        }

        public override void Clear()
        {
            _sessionValues.Clear();
        }

        public override void Remove(string key)
        {
            _sessionValues.Remove(key);
        }
    }
}