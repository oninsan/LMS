import React, { createContext, useState, useEffect } from "react";

export const LoggedUserContext = createContext();

export const LoggedUserProvider = ({ children }) => {
  const [loggedUser, setLoggedUser] = useState({
    role: sessionStorage.getItem("role"),
    idnumber: sessionStorage.getItem("idnumber"),
    login_method: sessionStorage.getItem("login_method"),
    firstName: sessionStorage.getItem("firstName"),
    lastName: sessionStorage.getItem("lastName"),
  });

  const setLoggedUserDetails = ({
    role,
    idnumber,
    login_method,
    firstName,
    lastName,
  }) => {
    setLoggedUser((prevUser) => ({
      ...prevUser,
      role: role,
      idnumber: idnumber,
      login_method,
    }));
    sessionStorage.setItem("role", role);
    sessionStorage.setItem("idnumber", idnumber);
    sessionStorage.setItem("login_method", login_method);
    sessionStorage.setItem("firstName", firstName);
    sessionStorage.setItem("lastName", lastName);
  };

  return (
    <LoggedUserContext.Provider
      value={{ loggedUser, setLoggedUser: setLoggedUserDetails }}
    >
      {children}
    </LoggedUserContext.Provider>
  );
};
