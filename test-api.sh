#!/bin/bash

# Test script for the Products and Categories API
BASE_URL="http://localhost:5000"

echo "🚀 Testing Products and Categories API"
echo "======================================="

echo
echo "1. Testing root endpoint..."
curl -s "$BASE_URL/" | echo

echo
echo "2. Getting all categories..."
curl -s "$BASE_URL/categories" | jq '.' 2>/dev/null || curl -s "$BASE_URL/categories"

echo
echo "3. Getting all products..."
curl -s "$BASE_URL/products" | jq '.' 2>/dev/null || curl -s "$BASE_URL/products"

echo
echo "4. Getting products by category (Computers - ID 2)..."
curl -s "$BASE_URL/products/category/2" | jq '.' 2>/dev/null || curl -s "$BASE_URL/products/category/2"

echo
echo "5. Creating a new category..."
curl -s -X POST "$BASE_URL/categories" \
  -H "Content-Type: application/json" \
  -d '{"name": "Test Category", "description": "A test category"}' | jq '.' 2>/dev/null || curl -s -X POST "$BASE_URL/categories" \
  -H "Content-Type: application/json" \
  -d '{"name": "Test Category", "description": "A test category"}'

echo
echo "6. Creating a new product..."
curl -s -X POST "$BASE_URL/products" \
  -H "Content-Type: application/json" \
  -d '{"name": "Test Product", "price": 99.99, "categoryId": 1}' | jq '.' 2>/dev/null || curl -s -X POST "$BASE_URL/products" \
  -H "Content-Type: application/json" \
  -d '{"name": "Test Product", "price": 99.99, "categoryId": 1}'

echo
echo "✅ API testing completed!"
echo
echo "Available endpoints:"
echo "GET    $BASE_URL/"
echo "GET    $BASE_URL/products"
echo "GET    $BASE_URL/products/{id}"
echo "GET    $BASE_URL/products/category/{categoryId}"
echo "POST   $BASE_URL/products"
echo "PUT    $BASE_URL/products/{id}"
echo "DELETE $BASE_URL/products/{id}"
echo "GET    $BASE_URL/categories"
echo "GET    $BASE_URL/categories/{id}"
echo "POST   $BASE_URL/categories"
echo "PUT    $BASE_URL/categories/{id}"
echo "DELETE $BASE_URL/categories/{id}"