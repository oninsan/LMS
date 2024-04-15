import { useState } from "react";
import { Outlet, useLocation, NavLink } from "react-router-dom";
import { Container, Row, Col, Nav, NavItem } from "reactstrap";
import NavigationBar from "./NavigationBar";
import { ToastContainer } from "react-toastify";
import "react-toastify/dist/ReactToastify.css";
import VerticalNavigation from "./VerticalNavigation";

function App() {
  const location = useLocation();
  const isLoginPage = location.pathname === "/";

  return (
    <Container fluid>
      <ToastContainer />
      {!isLoginPage && (
        <Row className="navigation-section">
          <NavigationBar />
        </Row>
      )}
      <Row>
        {!isLoginPage ? (
          <>
            {/* Menu */}
            <VerticalNavigation />
          </>
        ) : (
          <Outlet />
        )}
      </Row>
    </Container>
  );
}

export default App;
