using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIProject
{
    public class APIResponseV1<T>
    {

        public APIResponseV1(string statusMessage, object dto)
        {
            this.status = statusMessage;
            this.data = (T)dto;
        }
        public APIResponseV1(object dto)
        {

            this.data = (T)dto;
        }

        public APIResponseV1()
        {

        }

        private string _status = "success";
        public string status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        public T data
        {
            get; set;
        }

        public override string ToString()
        {
            //TODO: convert this to handle other types (serializer instead of this)
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{");
            sb.AppendFormat("\"status\":\"{0}\",", this.status);
            string dataString = this.data == null ? "{}" : this.data.ToString();
            sb.AppendFormat("\"data\":\"{0}\"", dataString);
            sb.Append("}");
            return sb.ToString();
        }
    }
}
