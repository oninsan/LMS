import React, { useContext, useState } from "react";
import { LoggedUserContext } from "./provider";
const formatDate = (date) => {
  return `${date.getFullYear()}-${(date.getMonth() + 1)
    .toString()
    .padStart(2, "0")}-${date.getDate().toString().padStart(2, "0")}`;
};

const formatTime = (date) => {
  let [hours, minutes, seconds] = [
    date.getHours(),
    date.getMinutes(),
    date.getSeconds(),
  ].map((val) => val.toString().padStart(2, "0"));
  let fractionalSeconds =
    date.getMilliseconds().toString().padStart(3, "0") + "0000";
  let formattedTime = `${hours}:${minutes}:${seconds}.${fractionalSeconds}`;
  return formattedTime;
};

export const useGetLoggedUser = () => {
  const { loggedUser } = useContext(LoggedUserContext);
  if (loggedUser === undefined) {
    throw new Error("useGlobalState must be used within a GlobalStateProvider");
  }
  return loggedUser;
};

export const useLogin = () => {
  const { loggedUser, setLoggedUser } = useContext(LoggedUserContext);
  const [creds, setCreds] = useState({
    idnumber: "",
    key: "",
    attendancedate: "",
    timein: "",
  });

  const setLoginCreds = (_name, value) => {
    setCreds((currentCreds) => ({
      ...currentCreds,
      [_name]: value,
      attendancedate: formatDate(new Date()),
      timein: formatTime(new Date()),
    }));
  };

  const onSubmit = async (e) => {
    let isSuccess = false;
    e.preventDefault();
    try {
      const res = await fetch(
        `http://localhost:5086/api/Client/NormalLogin?idnumber=${creds.idnumber}&userkey=${creds.key}&attendancedate=${creds.attendancedate}&timein=${creds.timein}`,
        {
          method: "POST",
        }
      );

      if (res.ok) {
        isSuccess = true;
        const data = await res.json();
        setLoggedUser(data);
        setCreds((currentCreds) => ({
          ...currentCreds,
          idnumber: "",
          key: "",
          attendancedate: "",
          timein: "",
        }));
      } else {
        console.error(res);
      }
    } catch (error) {
      console.error(error);
    }
    console.log(isSuccess);
    return isSuccess;
  };

  return [creds, setLoginCreds, onSubmit];
};
