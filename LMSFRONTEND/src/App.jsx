import { useState } from "react";
import { Outlet } from "react-router-dom";
import { Container, Row } from "reactstrap";
import NavigationBar from "./NavigationBar";
function App() {
  return (
    <Container fluid>
      <Row className="navigation-section border">
        <NavigationBar />
      </Row>
      <Row>
        <Outlet />
      </Row>
    </Container>
  );
}

export default App;
