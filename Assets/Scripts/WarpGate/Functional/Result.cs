using System;

namespace WarpGate.Functional
{
    public struct Result<T>
    {
        private T m_value;
        private Error m_error;

        private bool m_success;

        public bool IsSuccess => m_success;

        public bool IsFailure => !m_success;

        public static Result<T> Ok(T value)
        {
            var r = new Result<T>
            {
                m_value = value,
                m_success = true
            };

            return r;
        }

        public static Result<T> Err(Error error)
        {
            var r = new Result<T>
            {
                m_error = error,
                m_success = false
            };
            
            return r;
        }

        public void Match(Action<T> onMatchedOk, Action<Error> onMatchedError)
        {
            MatchOk(onMatchedOk);
            MatchError(onMatchedError);
        }

        public void MatchOk(Action<T> onMatched)
        {
            if (onMatched == null) throw new Exception("Match callback is null!");
            if (IsSuccess) onMatched(m_value);
        }

        public void MatchError(Action<Error> onMatched)
        {
            if (onMatched == null) throw new Exception("Match callback is null!");
            if (IsFailure) onMatched(m_error);
        }
    }

    public struct Unit
    {
        public static Unit Value = new Unit();
    }
}