import { Routes, Route } from "react-router-dom";
import App from "./App";
import HomePage from "./routes/HomePage";
import LoginPage from "./routes/LoginPage";
const AppRouter = () => {
  return (
    <>
      <Routes>
        <Route path="" element={<App />}>
          <Route index element={<LoginPage />} />
          <Route path="/home" element={<HomePage />} />
        </Route>
      </Routes>
    </>
  );
};

export default AppRouter;
