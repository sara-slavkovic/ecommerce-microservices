import { BrowserRouter, Routes, Route } from "react-router-dom";
import Main from "./pages/Main";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Home from "./pages/Home";
import Cart from "./pages/Cart";
import Checkout from "./pages/Checkout";
import Orders from "./pages/Orders";
import ProductDetails from "./pages/ProductDetails";
import EditProfile from "./pages/EditProfile";
import AdminProducts from "./pages/AdminProducts";
import AdminRoute from "./components/AdminRoute";
import ProtectedRoute from "./components/ProtectedRoute";
import Layout from "./components/Layout";
import { ToastProvider } from "./hooks/useToast";
import { CartProvider } from "./hooks/useCart";
import { ConfirmProvider } from "./hooks/useConfirm";

function App() {
  return (
    <ToastProvider>
      <BrowserRouter>
        <CartProvider>
          <ConfirmProvider>
            <Routes>
              <Route path="/" element={<Main />} />
              <Route path="/login" element={<Login />} />
              <Route path="/register" element={<Register />} />
              <Route
                element={
                  <ProtectedRoute>
                    <Layout />
                  </ProtectedRoute>
                }
              >
                <Route path="/home" element={<Home />} />
                <Route path="/cart" element={<Cart />} />
                <Route path="/checkout" element={<Checkout />} />
                <Route path="/orders" element={<Orders />} />
                <Route path="/product/:id" element={<ProductDetails />} />
                <Route path="/profile/edit" element={<EditProfile />} />
              </Route>
              <Route
                path="/admin/products"
                element={
                  <AdminRoute>
                    <Layout>
                      <AdminProducts />
                    </Layout>
                  </AdminRoute>
                }
              />
            </Routes>
          </ConfirmProvider>
        </CartProvider>
      </BrowserRouter>
    </ToastProvider>
  );
}

export default App;
