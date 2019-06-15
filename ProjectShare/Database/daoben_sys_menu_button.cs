namespace ProjectShare.Database
{
    public class daoben_sys_menu_button
    {
        /// <summary>
        /// id
        /// </summary>		
        public string id { get; set; }		
        public string encode { get; set; }
        /// <summary>
        /// name
        /// </summary>		
        public string name { get; set; }
        /// <summary>
        /// menu_id
        /// </summary>		
        public string menu_code { get; set; }
        /// <summary>
        /// sort
        /// </summary>		
        public string sort { get; set; }
        /// <summary>
        /// url
        /// </summary>		
        //[DisplayFormat(ConvertEmptyStringToNull = false)]
        public string url { get; set; }
        /// <summary>
        /// 参数(用&分开)
        /// </summary>		
        //[DisplayFormat(ConvertEmptyStringToNull = false)]
        public string param { get; set; }
        /// <summary>
        /// 0-无效，1-有效
        /// </summary>		
        public bool active { get; set; }
    }
}