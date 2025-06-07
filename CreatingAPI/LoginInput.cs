using System;
using System.Collections.Generic;

public class APIModel{

}

public class UserAccount{
    public string username;
    public string email;
    public DateTime createdAt;


    public bool IsEmailValid(){
        if(email.Contains("@") && email.Contains(".")){
            return true;
        }
        else{
            return false;
        }
    }

    public UserAccount (string usernameoffered, string emailoffered){
        username = usernameoffered;
        email = emailoffered;
        createdAt = DateTime.UtcNow;
    }
}
