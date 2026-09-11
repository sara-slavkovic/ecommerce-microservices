import { Outlet } from "react-router-dom";
import Navbar from "./Navbar";

function Layout({ children }) {
  return (
    <div style={{ minHeight: "100vh" }}>
      <Navbar />
      {children || <Outlet />}
    </div>
  );
}

export default Layout;
