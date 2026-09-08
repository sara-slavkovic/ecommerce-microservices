import { useEffect, useState } from "react";
import Navbar from "../components/Navbar";
import ProductCard from "../components/ProductCard";
import { getAllProducts } from "../api/catalogService";
import { addItemToCart } from "../api/cartService";
import { getErrorMessage } from "../api/errorHandling";
import { useAuth } from "../hooks/useAuth";
import { useToast } from "../hooks/useToast";

function Home() {
  const { user } = useAuth();
  const showToast = useToast();
  const [products, setProducts] = useState([]);
  const [error, setError] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    getAllProducts()
      .then(setProducts)
      .catch((err) =>
        setError(
          getErrorMessage(
            err,
            "Products are currently unavailable. Thanks for your patience!",
          ),
        ),
      )
      .finally(() => setLoading(false));
  }, []);

  const handleAddToCart = async (product) => {
    try {
      await addItemToCart(user.id, product.id, 1);
      showToast(`${product.name} added to cart!`, "success");
    } catch (err) {
      showToast(getErrorMessage(err, "Failed to add to cart."), "error");
    }
  };

  if (loading) {
    return (
      <div style={{ minHeight: "100vh" }}>
        <Navbar />
        <div style={{ padding: "4rem 2rem", textAlign: "center" }}>
          Loading products...
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div style={{ minHeight: "100vh" }}>
        <Navbar />
        <div
          style={{
            maxWidth: "500px",
            margin: "80px auto",
            padding: "30px",
            backgroundColor: "var(--card-bg)",
            borderRadius: "8px",
            textAlign: "center",
          }}
        >
          <h2 style={{ marginTop: 0 }}>Oops!</h2>
          <p>{error}</p>
        </div>
      </div>
    );
  }

  return (
    <div style={{ minHeight: "100vh" }}>
      <Navbar />
      <div style={{ padding: "2rem", maxWidth: "1200px", margin: "0 auto" }}>
        <h1
          style={{
            textAlign: "center",
            fontSize: "2.5rem",
            marginBottom: "2rem",
          }}
        >
          Our Products
        </h1>
        {error && (
          <p style={{ textAlign: "center", color: "#b33" }}>Error: {error}</p>
        )}
        {!error && products.length === 0 && (
          <p style={{ textAlign: "center" }}>Loading products...</p>
        )}
        <div
          style={{
            display: "grid",
            gridTemplateColumns: "repeat(auto-fill, minmax(230px, 1fr))",
            gap: "20px",
          }}
        >
          {products.map((p) => (
            <ProductCard key={p.id} product={p} onAddToCart={handleAddToCart} />
          ))}
        </div>
      </div>
    </div>
  );
}

export default Home;
