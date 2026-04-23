using ADay15.NET.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ADay15.NET.Infrastructure.Commons
{
    public class R<T>
    {
        public int Code {  get; set; }
        public string Msg {  get; set; }
        public T Data { get; set; }

        public static R<T> Sucess() 
        {
            return new R<T>
            {
                Code = (int)ResponseCode.Success,
                Msg = "操作成功",
                Data = default
            };
        }


        public static R<T> Sucess(T data) 
        {
            return new R<T>
            {
                Code = (int)ResponseCode.Success,
                Msg = "操作成功",
                Data = data
            };
        }

        public static R<T> Sucess(string msg,T data) 
        {
            return new R<T>
            {
                Code = (int)ResponseCode.Success,
                Msg = msg,
                Data = data
            };
        }

        public static R<T> Fail(string msg) 
        {
            return new R<T>
            {
                Code = (int)ResponseCode.ServerError,
                Msg = msg,
                Data = default
            };
        }

        public static R<T> Fail(ResponseCode code, string msg) 
        {
            return new R<T>
            {
                Code = (int)code,
                Msg = msg,
                Data = default
            };
        }
    }
}
