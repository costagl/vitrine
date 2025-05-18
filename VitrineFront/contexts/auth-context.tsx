"use client"

import { createContext, useContext, useState, useEffect, type ReactNode } from "react"

interface AuthContextType {
  isAuthenticated: boolean
  token: string | null
  login: (token: string) => void
  logout: () => void
}

const AuthContext = createContext<AuthContextType | undefined>(undefined)

export function AuthProvider({ children }: { children: ReactNode }) {
  const [isAuthenticated, setIsAuthenticated] = useState(false)
  const [token, setToken] = useState<string | null>(null)

  useEffect(() => {
    // Verificar se há um token no localStorage quando o componente é montado
    const storedToken = localStorage.getItem("token")
    if (storedToken) {
      setToken(storedToken)
      setIsAuthenticated(true)
    }
    const testarAPI = async () => {
      try {
        const resposta = await fetch("https://f940-177-131-178-100.ngrok-free.app/api/Account/test")
        if (resposta.ok) {
          console.log("✅ API funcionando!")
        } else {
          console.log("❌ API respondeu com erro:", resposta.status)
        }
      } catch (error) {
        console.log("❌ Erro ao conectar com a API:", (error as Error).message)
      }
  }

  testarAPI()
  }, [])

  const login = (newToken: string) => {
    localStorage.setItem("token", newToken)
    setToken(newToken)
    setIsAuthenticated(true)
  }

  const logout = async () => {
    try {
      const resposta = await fetch("https://f940-177-131-178-100.ngrok-free.app/api/Account/logout", {
        method: "POST",
        credentials: "include",
      })

      if (resposta.ok) {
        localStorage.removeItem("token")
        setToken(null)
        setIsAuthenticated(false)
      } else {
        console.error("Logout falhou com status:", resposta.status)
      }
    } catch (error) {
      console.error("Erro ao fazer logout no backend:", error)
    }
  }

  return <AuthContext.Provider value={{ isAuthenticated, token, login, logout }}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const context = useContext(AuthContext)
  if (context === undefined) {
    throw new Error("useAuth deve ser usado dentro de um AuthProvider")
  }
  return context
}
