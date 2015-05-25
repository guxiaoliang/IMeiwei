using System;
namespace IMeiWei.Model
{
    /// <summary>
    /// ticket:实体类(属性说明自动提取数据库字段的描述信息)
    /// </summary>
    [Serializable]
    public partial class ticket
    {
        public ticket()
        { }
        #region Model
        private long _id;
        private int _ticketnumber;
        private int _ticketamount;
        private DateTime _limittime;
        private DateTime _createtime;
        private string _createby;
        private string _ticketreferenceid;
        /// <summary>
        /// auto_increment
        /// </summary>
        public long Id
        {
            set { _id = value; }
            get { return _id; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int TicketNumber
        {
            set { _ticketnumber = value; }
            get { return _ticketnumber; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int TicketAmount
        {
            set { _ticketamount = value; }
            get { return _ticketamount; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime LimitTime
        {
            set { _limittime = value; }
            get { return _limittime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime
        {
            set { _createtime = value; }
            get { return _createtime; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string CreateBy
        {
            set { _createby = value; }
            get { return _createby; }
        }
        /// <summary>
        /// 
        /// </summary>
        public string TicketReferenceId
        {
            set { _ticketreferenceid = value; }
            get { return _ticketreferenceid; }
        }
        #endregion Model

    }
}

