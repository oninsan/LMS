import { Routes, Route } from "react-router-dom";
import App from "./App";
import HomePage from "./routes/HomePage";
import LoginPage from "./routes/LoginPage";
import UsersPage from "./routes/Users";
import BooksPage from "./routes/BooksPage";
import EquipmentsPage from "./routes/EquipmentsPage";
import BorrowedBooksPage from "./routes/BarrowedBooksPage";
import BorrowedEquipmentsPage from "./routes/BorrowedEquipmentsPage";
const AppRouter = () => {
  return (
    <>
      <Routes>
        <Route path="/" element={<App />}>
          <Route index element={<LoginPage />} />
          <Route path="/home" element={<HomePage />} />
          <Route path="/users" element={<UsersPage />} />
          <Route path="/books" element={<BooksPage />} />
          <Route path="/equipment" element={<EquipmentsPage />} />
          <Route path="/borrowed-books" element={<BorrowedBooksPage />} />
          <Route path="/borrowed-equipments" element={<HomePage />} />
        </Route>
      </Routes>
    </>
  );
};

export default AppRouter;
