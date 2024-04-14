import { Row, Col, Container } from "reactstrap";
import { NavLink } from "react-router-dom";
import { FontAwesomeIcon as Icon } from "@fortawesome/react-fontawesome";
import { faUser } from "@fortawesome/free-solid-svg-icons";
import logo from "./assets/img/logo.png";
import "./assets/css/NavigationBarCss.css";
import {
  faBars,
  faNewspaper,
  faSearch,
} from "@fortawesome/free-solid-svg-icons";

const NavigationBar = () => {
  return (
    <>
      <Col className="" lg={4} md={3} sm={2} xs={2}>
        <Container className="d-none d-md-block">
          <Row className="align-items-center justify-content-center">
            <Col xl={2} lg={2} sm={4}>
              {/* <Icon icon={logo} className="fs-3" /> */}
              <img src={logo} alt="logo" className="logo ms-xl-4" />
            </Col>
            <Col className="mt-3" xl={10} lg={10} sm={8}>
              <p className="fw-bold">LCCTO-LMS</p>
            </Col>
          </Row>
        </Container>
        <Container className="text-center mx-auto d-lg-none d-md-none d-sm-block d-xs-block">
          <Icon icon={faBars} className="" style={{ fontSize: 40 }} />
        </Container>
      </Col>
      <Col className="" lg={8} md={9} sm={10} xs={10}>
        <Row className="mt-2">
          <Col xl={11} lg={10} md={10} sm={10} xs={10}></Col>
          <Col xl={1} lg={2} md={2} sm={2} xs={2}>
            <Row className="mt-lg-1">
              <Col xl={7} lg={5} md={6} sm={7} xs={2}>
                <Icon icon={faUser} className="fs-3 ms-lg-4 ms-sm-4" />
              </Col>
              <Col xl={2} lg={3} md={3} sm={5} xs={2}>
                <select name="" id="" className="user-dropdown">
                  <option value=""></option>
                </select>
              </Col>
            </Row>
          </Col>
        </Row>
      </Col>
    </>
  );
};

export default NavigationBar;
