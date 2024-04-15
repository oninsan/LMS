import { Row, Col, Nav, NavItem } from "reactstrap";
import { Outlet, NavLink } from "react-router-dom";
import { FontAwesomeIcon as Icon } from "@fortawesome/react-fontawesome";
import {
  faUser,
  faHome,
  faBook,
  faBookOpen,
  faLaptop,
  faHouseLaptop,
} from "@fortawesome/free-solid-svg-icons";
const menuItems = [
  { path: "/home", label: "Home", color: "#007bff", icon: faHome },
  { path: "/users", label: "Users", color: "#28a745", icon: faUser },
  { path: "/books", label: "Books", color: "#ffc107", icon: faBook },
  { path: "/equipment", label: "Equipment", color: "#ffc107", icon: faLaptop },
  { path: "/borrowed-books", label: "Borrowed Books", icon: faBookOpen },
  {
    path: "/borrowed-equipments",
    label: "Borrowed Equipment",
    icon: faHouseLaptop,
  },
];
const VerticalNavigation = () => {
  return (
    <>
      <Col className="vert-nav-section" xl={2} lg={3} md={3}>
        <Nav vertical>
          {menuItems.map((item, index) => (
            <NavItem key={index} className="nav-item-custom my-3">
              <NavLink to={item.path} className="nav-link text-light">
                <Row>
                  <Col lg={3} className="text-end">
                    <Icon icon={item.icon} style={{ fontSize: ".9rem" }} />
                  </Col>
                  <Col lg={9}>{item.label}</Col>
                </Row>
              </NavLink>
            </NavItem>
          ))}
        </Nav>
      </Col>
      <Col className="border" xl={10} lg={9} md={9}>
        <Outlet />
      </Col>
    </>
  );
};

export default VerticalNavigation;
