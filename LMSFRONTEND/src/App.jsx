import { useState } from "react";
import { Outlet, useLocation } from "react-router-dom";
import { Container, Row } from "reactstrap";
import NavigationBar from "./NavigationBar";
function App() {
  const location = useLocation();
  const isLoginPage = location.pathname === "/";
  return (
    <Container fluid>
      {!isLoginPage && (
        <Row className="navigation-section border">
          <NavigationBar />
        </Row>
      )}
      <Row>
        <Outlet />
      </Row>
    </Container>
  );
}

export default App;
