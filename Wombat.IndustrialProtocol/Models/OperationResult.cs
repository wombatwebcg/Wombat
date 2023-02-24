using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Wombat.IndustrialProtocol
{




    /// <summary>
    /// 操作结果的类，只带有成功标志和错误信息 -> The class that operates the result, with only success flags and error messages
    /// </summary>
    /// <remarks>
    /// 当 <see cref="IsSuccess"/> 为 True 时，忽略 <see cref="Message"/> 及 <see cref="ErrorCode"/> 的值
    /// </remarks>
    /// 
    public class OperationResult
    {
        #region Constructor

        /// <summary>
        /// 实例化一个默认的结果对象
        /// </summary>
        /// 

        public OperationResult()
        {
        }



        /// <summary>
        /// 指示本次访问是否成功
        /// </summary>
        public bool IsSuccess { get; set; } = true;



        private string _message;

        /// <summary>
        /// 具体的错误描述
        /// </summary>
        /// 
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                AddMessage2List();
            }
        }

        /// <summary>
        /// 具体的错误代码
        /// </summary>
        /// 
        public int ErrorCode { get; set; } = 10000;


        /// <summary>
        /// 详细异常
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// 耗时（毫秒）
        /// </summary>
        public double? TimeConsuming { get; private set; }

        public string Requst { get; set; }

        /// <summary>
        /// 响应报文
        /// </summary>
        public string Response { get; set; }

        /// <summary>
        /// 请求报文2
        /// </summary>
        public string Requst2 { get; set; }

        /// <summary>
        /// 响应报文2
        /// </summary>
        public string Response2 { get; set; }

        /// <summary>
        /// 结束时间统计
        /// </summary>
        public OperationResult EndTime()
        {
            TimeConsuming = (DateTime.Now - InitialTime).TotalMilliseconds;
            return this;
        }

        public OperationResult CreatFailureEndTime()
        {
            IsSuccess = false;           
            return EndTime();

        }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime InitialTime { get; protected set; } = DateTime.Now;


        /// <summary>
        /// 异常集合
        /// </summary>
        public List<string> MessageList { get; private set; } = new List<string>();

        /// <summary>
        /// 设置异常信息和Succeed状态
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public OperationResult SetInfo(OperationResult result)
        {
            IsSuccess = result.IsSuccess;
            Message = result.Message;
            ErrorCode = result.ErrorCode;
            Exception = result.Exception;
            foreach (var err in result.MessageList)
            {
                if (!MessageList.Contains(err))
                    MessageList.Add(err);
            }
            return this;
        }


        /// <summary>
        /// 添加异常到异常集合
        /// </summary>
        public void AddMessage2List()
        {
            if (!MessageList.Contains(Message))
            {
                if (Message != null & Message != string.Empty)
                    MessageList.Add(Message);

            }
        }

        public static OperationResult Assignment(OperationResult orgin)
        {
            var newOperationValue = new OperationResult()
            {
                IsSuccess = orgin.IsSuccess,
                ErrorCode = orgin.ErrorCode,
                Message = orgin.Message,
                Exception = orgin.Exception,
                InitialTime = orgin.InitialTime,
                Requst = orgin.Requst,
                Requst2 = orgin.Requst2,
                Response = orgin.Response,
                Response2 = orgin.Response2
            };
            foreach (var message in orgin.MessageList)
            {
                if (message != null & message != string.Empty)
                    newOperationValue.MessageList.Add(message);
            }
            return newOperationValue;
        }


        #endregion

        #region Static Method

        /*****************************************************************************************************
         * 
         *    主要是方便获取到一些特殊状态的结果对象
         * 
         ******************************************************************************************************/








    }


    /// <summary>
    /// 操作结果的泛型类，允许带一个用户自定义的泛型对象，推荐使用这个类
    /// </summary>
    /// <typeparam name="T">泛型类</typeparam>
    /// 

    public class OperationResult<T> : OperationResult
    {
        #region Constructor

        /// <summary>
        /// 实例化一个默认的结果对象
        /// </summary>
        public OperationResult() : base()
        {
        }

        /// <summary>
        /// 使用指定的消息实例化一个默认的结果对象
        /// </summary>
        /// <param name="msg">错误消息</param>
        public OperationResult(T data) 
        {
            Value = data;
        }

        /// <summary>
        /// 使用指定的消息实例化一个默认的结果对象
        /// </summary>
        /// <param name="msg">错误消息</param>
        public OperationResult(OperationResult result)
        {
            var orgin = Assignment(result);
            this.ErrorCode = orgin.ErrorCode;
            this.Exception = orgin.Exception;
            this.InitialTime = orgin.InitialTime;
            this.IsSuccess = orgin.IsSuccess;
            this.Message = orgin.Message;
            foreach (var message in orgin.MessageList)
            {
                if (message != null & message != string.Empty)
                    this.MessageList.Add(message);
            }
            this.Requst = orgin.Requst;
            this.Requst2 = orgin.Requst2;
            this.Response = orgin.Response;
            this.Response2 = orgin.Response2;
        }

        /// <summary>
        /// 使用指定的消息实例化一个默认的结果对象
        /// </summary>
        /// <param name="msg">错误消息</param>
        public OperationResult(OperationResult result,T data)
        {
            var orgin = Assignment(result);
            this.ErrorCode = orgin.ErrorCode;
            this.Exception = orgin.Exception;
            this.InitialTime = orgin.InitialTime;
            this.IsSuccess = orgin.IsSuccess;
            this.Message = orgin.Message;
            foreach (var message in orgin.MessageList)
            {
                if(message!=null&message!=string.Empty)
                this.MessageList.Add(message);
            }
            this.Requst = orgin.Requst;
            this.Requst2 = orgin.Requst2;
            this.Response = orgin.Response;
            this.Response2 = orgin.Response2;
            Value = data;
        }

        /// <summary>
        /// 请求报文
        /// </summary>



        /// <summary>
        /// 用户自定义的泛型数据
        /// </summary>
        public T Value { get; set; }


        /// <summary>
        /// 结束时间统计
        /// </summary>
        public new OperationResult<T> EndTime()
        {
            base.EndTime();
            return this;

        }

        public new OperationResult<T> CreatFailureEndTime()
        {
            IsSuccess = false;
            base.EndTime();
            return this;

        }

        #endregion

        /// <summary>
        /// 设置异常信息和Succeed状态
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public OperationResult<T> SetInfo(OperationResult<T> result)
        {
            Value = result.Value;
            base.SetInfo(result);
            return this;
        }

        /// <summary>
        /// 设置异常信息和Succeed状态
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public new OperationResult<T> SetInfo(OperationResult result)
        {
            base.SetInfo(result);
            return this;
        }

    }
    /// <summary>
    /// 操作结果的泛型类，允许带两个用户自定义的泛型对象，推荐使用这个类
    /// </summary>
    /// <typeparam name="T1">泛型类</typeparam>
    /// <typeparam name="T2">泛型类</typeparam>
    public class OperationResult<T1, T2> : OperationResult
    {
        #region Constructor

        /// <summary>
        /// 实例化一个默认的结果对象
        /// </summary>
        public OperationResult(OperationResult result) : base()
        {
            var orgin = Assignment(result);
            this.ErrorCode = orgin.ErrorCode;
            this.Exception = orgin.Exception;
            this.InitialTime = orgin.InitialTime;
            this.IsSuccess = orgin.IsSuccess;
            this.Message = orgin.Message;
            foreach (var message in orgin.MessageList)
            {
                if (message != null & message != string.Empty)
                    this.MessageList.Add(message);
            }
            this.Requst = orgin.Requst;
            this.Requst2 = orgin.Requst2;
            this.Response = orgin.Response;
            this.Response2 = orgin.Response2;

        }

        /// <summary>
        /// 使用指定的消息实例化一个默认的结果对象
        /// </summary>
        /// <param name="msg">错误消息</param>
        public OperationResult(OperationResult result, T1 data1,T2 data2)
        {
            var orgin = Assignment(result);
            this.ErrorCode = orgin.ErrorCode;
            this.Exception = orgin.Exception;
            this.InitialTime = orgin.InitialTime;
            this.IsSuccess = orgin.IsSuccess;
            this.Message = orgin.Message;
            foreach (var message in orgin.MessageList)
            {
                if (message != null & message != string.Empty)
                    this.MessageList.Add(message);
            }
            this.Requst = orgin.Requst;
            this.Requst2 = orgin.Requst2;
            this.Response = orgin.Response;
            this.Response2 = orgin.Response2;
            Value1 = data1;
            Value2 = data2;

        }


        #endregion

        /// <summary>
        /// 用户自定义的泛型数据1
        /// </summary>
        public T1 Value1 { get; set; }

        /// <summary>
        /// 用户自定义的泛型数据2
        /// </summary>
        public T2 Value2 { get; set; }

        /// <summary>
        /// 结束时间统计
        /// </summary>
        public new OperationResult<T1, T2> EndTime()
        {
            base.EndTime();
            return this;

        }

        public new OperationResult<T1, T2> CreatFailureEndTime()
        {
            IsSuccess = false;
            base.EndTime();
            return this;

        }


        /// <summary>
        /// 设置异常信息和Succeed状态
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public OperationResult<T1,T2> SetInfo(OperationResult<T1,T2> result)
        {
            Value1 = result.Value1;
            Value2 = result.Value2;
            base.SetInfo(result);
            return this;
        }

        /// <summary>
        /// 设置异常信息和Succeed状态
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public new OperationResult<T1, T2> SetInfo(OperationResult result)
        {
            base.SetInfo(result);
            return this;
        }

    }
    #endregion

}





