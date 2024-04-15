import {
  Row,
  Col,
  Container,
  Dropdown,
  DropdownMenu,
  DropdownItem,
  DropdownToggle,
} from "reactstrap";
import { NavLink } from "react-router-dom";
import { FontAwesomeIcon as Icon } from "@fortawesome/react-fontawesome";
import {
  faUser,
  faUserAlt,
  faUserCircle,
} from "@fortawesome/free-solid-svg-icons";
import logo from "./assets/img/logo.png";
import "./assets/css/NavigationBarCss.css";
import { faBars } from "@fortawesome/free-solid-svg-icons";
import { useContext, useState } from "react";
import { LoggedUserContext } from "./hooks/provider";

const NavigationBar = () => {
  const { loggedUser } = useContext(LoggedUserContext);
  const [dropdownOpen, setDropdownOpen] = useState(false);

  const toggle = () => setDropdownOpen((prevState) => !prevState);
  console.log(loggedUser);
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
              <p className="fw-bold text-light">LCCTO-LMS</p>
            </Col>
          </Row>
        </Container>
        <Container className="text-center mx-auto d-lg-none d-md-none d-sm-block d-xs-block">
          <Icon icon={faBars} className="" style={{ fontSize: 40 }} />
        </Container>
      </Col>
      <Col className="" lg={8} md={9} sm={10} xs={10}>
        <Row className="">
          <Col xl={9} lg={9} md={8} sm={9} xs={8}></Col>
          <Col xl={2} lg={3} md={4} sm={3} xs={4}>
            <Row className="mt-lg-1">
              <Col xl={7} lg={5} md={6} sm={3} xs={3} className="mt-2 mt-md-3">
                <Icon
                  icon={faUserCircle}
                  className="fs-3 ms-lg-4 ms-md-5 ms-xl-5 text-light"
                />
              </Col>
              <Col xl={5} lg={3} md={3} sm={7} xs={2}>
                <Dropdown
                  isOpen={dropdownOpen}
                  toggle={toggle}
                  size="sm"
                  className="mt-1 my-md-3"
                  color="danger"
                >
                  <DropdownToggle caret className="nv-btn">
                    {loggedUser.lastName}
                  </DropdownToggle>
                  <DropdownMenu>
                    <DropdownItem header>Header</DropdownItem>
                    <DropdownItem disabled>Action</DropdownItem>
                    <DropdownItem>Another Action</DropdownItem>
                    <DropdownItem divider />
                    <DropdownItem>Logout</DropdownItem>
                  </DropdownMenu>
                </Dropdown>
              </Col>
            </Row>
          </Col>
        </Row>
      </Col>
    </>
  );
};

export default NavigationBar;
