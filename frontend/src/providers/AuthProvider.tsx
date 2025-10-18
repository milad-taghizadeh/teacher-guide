import React, { useCallback, useEffect, useMemo, useState } from "react";
import { apiClient, getAuthToken, setAuthToken } from "../api/client";

interface AuthContextValue {
  username: string | null;
  token: string | null;
  login: (username: string, password: string) => Promise<void>;
  logout: () => void;
}

export const AuthContext = React.createContext<AuthContextValue | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [username, setUsername] = useState<string | null>(null);
  const [token, setToken] = useState<string | null>(null);

  useEffect(() => {
    const savedToken = getAuthToken();
    if (savedToken) {
      setToken(savedToken);
      const savedUsername = localStorage.getItem("tg_username");
      if (savedUsername) {
        setUsername(savedUsername);
      }
    }
  }, []);

  const login = useCallback(async (usernameInput: string, password: string) => {
    const response = await apiClient.post("/api/auth/login", { username: usernameInput, password });
    const data = response.data as { token: string; username: string };
    setAuthToken(data.token);
    localStorage.setItem("tg_username", data.username);
    setUsername(data.username);
    setToken(data.token);
  }, []);

  const logout = useCallback(() => {
    setAuthToken(null);
    localStorage.removeItem("tg_username");
    setUsername(null);
    setToken(null);
  }, []);

  const value = useMemo(() => ({ username, token, login, logout }), [username, token, login, logout]);

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
};
