package main

import (
	"io"
	"log"
	"net/http"
)

func getSamplingRulesHandler(w http.ResponseWriter, r *http.Request) {
	// Only respond to POST requests for /GetSamplingRules
	if r.Method != http.MethodPost || r.URL.Path != "/GetSamplingRules" {
		http.NotFound(w, r)
		log.Printf("Received unhandled request: Method=%s, Path=%s, From=%s", r.Method, r.URL.Path, r.RemoteAddr)
		return
	}

	log.Printf("Received HTTP GET request on /GetSamplingRules from %s", r.RemoteAddr)
	log.Printf("Request Host: %s", r.Host)
	log.Printf("Request User-Agent: %s", r.UserAgent())
	log.Printf("Request Headers:")
	for name, values := range r.Header {
		for _, value := range values {
			log.Printf("  %s: %s", name, value)
		}
	}

	// Read the request body (if any)
	if r.ContentLength > 0 {
		body, err := io.ReadAll(r.Body)
		if err != nil {
			log.Printf("Error reading request body: %v", err)
		} else {
			log.Printf("Request Body: %s", string(body))
		}
	}

	// Send a 200 OK response
	w.WriteHeader(http.StatusOK)
	_, err := w.Write([]byte("OK"))
	if err != nil {
		log.Printf("Error writing response: %v", err)
	}
	log.Printf("Sent 200 OK response to %s for /GetSamplingRules", r.RemoteAddr)
}

func main() {
	// Register the handler for the specific URL path
	http.HandleFunc("/GetSamplingRules", getSamplingRulesHandler)

	// Start the HTTP server on port 2000
	port := ":2000"
	log.Printf("Server starting on port %s...", port)
	err := http.ListenAndServe(port, nil)
	if err != nil {
		log.Fatalf("Server failed to start: %v", err)
	}
}
