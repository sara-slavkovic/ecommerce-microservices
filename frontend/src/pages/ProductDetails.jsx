import { useEffect, useState } from "react";
import { useParams, useNavigate } from "react-router-dom";
import QuantityInput from "../components/QuantityInput";
import Spinner from "../components/Spinner";
import { getProductById, getImageUrl } from "../api/catalogService";
import { addItemToCart } from "../api/cartService";
import { getErrorMessage } from "../api/errorHandling";
import { useAuth } from "../hooks/useAuth";
import { useToast } from "../hooks/useToast";
import { useCart } from "../hooks/useCart";

function ProductDetails() {
  const { id } = useParams();
  const navigate = useNavigate();
  const { user } = useAuth();
  const showToast = useToast();
  const { refreshCartCount } = useCart();

  const [product, setProduct] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [quantity, setQuantity] = useState(1);
  const [backHovered, setBackHovered] = useState(false);

  useEffect(() => {
    getProductById(id)
      .then(setProduct)
      .catch((err) =>
        setError(
          getErrorMessage(err, "This product is currently unavailable."),
        ),
      )
      .finally(() => setLoading(false));
  }, [id]);

  const handleAddToCart = async () => {
    try {
      await addItemToCart(user.id, product.id, quantity);
      showToast(`${product.name} added to cart!`, "success");
      refreshCartCount();
    } catch (err) {
      showToast(getErrorMessage(err, "Failed to add to cart."), "error");
    }
  };

  if (loading) {
    return (
      <div style={{ minHeight: "100vh" }}>
        <Spinner text="Loading product..." />
      </div>
    );
  }

  if (error) {
    return (
      <div style={{ minHeight: "100vh" }}>
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
      <div style={{ padding: "2rem", maxWidth: "1000px", margin: "0 auto" }}>
        <button
          onClick={() => navigate("/home")}
          onMouseEnter={() => setBackHovered(true)}
          onMouseLeave={() => setBackHovered(false)}
          style={{
            marginBottom: "1.5rem",
            fontSize: "0.85rem",
            padding: "6px 14px",
            backgroundColor: backHovered ? "var(--text-main)" : "transparent",
            color: backHovered ? "var(--bg-color)" : "var(--text-main)",
            border: "1px solid var(--text-main)",
            transition: "background-color 0.15s ease, color 0.15s ease",
          }}
        >
          ← Back to Shop
        </button>

        <div style={{ display: "flex", gap: "3rem", flexWrap: "wrap" }}>
          <div style={{ flex: "1 1 350px" }}>
            <img
              src={getImageUrl(product.imageUrl)}
              alt={product.name}
              style={{
                width: "100%",
                aspectRatio: "1 / 1",
                objectFit: "cover",
                borderRadius: "10px",
              }}
            />
          </div>

          <div style={{ flex: "1 1 350px" }}>
            <p style={{ margin: "0 0 5px 0", fontStyle: "italic" }}>
              {product.brand}
            </p>
            <h1 style={{ margin: "0 0 15px 0" }}>{product.name}</h1>
            <h2 style={{ margin: "0 0 20px 0" }}>
              ${product.price.toFixed(2)}
            </h2>

            {product.categoryName && (
              <p style={{ margin: "0 0 15px 0", fontSize: "0.9rem" }}>
                Category: {product.categoryName}
              </p>
            )}

            <p style={{ lineHeight: 1.6, marginBottom: "25px" }}>
              {product.description}
            </p>

            <div style={{ display: "flex", alignItems: "center", gap: "15px" }}>
              <QuantityInput initialQuantity={1} onChange={setQuantity} />
              <button
                onClick={handleAddToCart}
                style={{ padding: "8px 22px", fontSize: "0.95rem" }}
              >
                Add to Cart
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}

export default ProductDetails;
