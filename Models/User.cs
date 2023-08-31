namespace JWTGenerator.Models
{
    public class User
    {
        public int UserId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; }    
        /// <summary>
        /// 
        /// </summary>
        public byte[] PasswordSalt { get; set; }    
        public byte[] HashedPassword { get; set; }
    }
}
