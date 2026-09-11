import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { getImageUrl } from "../api/catalogService";

function ProductCard({ product, onAddToCart }) {
  const navigate = useNavigate();
  const [hovered, setHovered] = useState(false);
  const imageUrl = getImageUrl(product.imageUrl);

  return (
    <div
      onClick={() => navigate(`/product/${product.id}`)}
      onMouseEnter={() => setHovered(true)}
      onMouseLeave={() => setHovered(false)}
      style={{
        backgroundColor: "var(--card-bg)",
        borderRadius: "8px",
        padding: "15px",
        textAlign: "center",
        boxShadow: hovered
          ? "0 10px 20px rgba(0,0,0,0.12)"
          : "0 4px 6px rgba(0,0,0,0.05)",
        cursor: "pointer",
        display: "flex",
        flexDirection: "column",
        transform: hovered ? "translateY(-4px)" : "translateY(0)",
        transition: "transform 0.2s ease, box-shadow 0.2s ease",
      }}
    >
      <img
        src={imageUrl}
        alt={product.name}
        style={{
          width: "100%",
          aspectRatio: "1 / 1",
          objectFit: "cover",
          borderRadius: "5px",
        }}
      />
      <h3 style={{ margin: "15px 0 5px 0" }}>{product.name}</h3>
      <p style={{ margin: "0 0 10px 0", fontStyle: "italic" }}>
        {product.brand}
      </p>
      <div style={{ marginTop: "auto" }}>
        <h3 style={{ margin: "0 0 15px 0" }}>${product.price.toFixed(2)}</h3>
        <button
          style={{ width: "100%" }}
          onClick={(e) => {
            e.stopPropagation();
            onAddToCart(product);
          }}
        >
          Add to Cart
        </button>
      </div>
    </div>
  );
}

export default ProductCard;
