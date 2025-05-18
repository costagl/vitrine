"use client"

import type React from "react"

import { useState } from "react"
import { useRouter } from "next/navigation"
import { useToast } from "@/hooks/use-toast"
import { useAuth } from "@/contexts/auth-context"
import Link from "next/link"
import { Button } from "@/components/ui/button"
import { Card, CardContent, CardDescription, CardFooter, CardHeader, CardTitle } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Checkbox } from "@/components/ui/checkbox"
import Navbar from "@/components/navbar"

export default function LoginPage() {
  const { toast } = useToast()
  const router = useRouter()
  const { login } = useAuth()
  const [isSubmitting, setIsSubmitting] = useState(false)
  const [formData, setFormData] = useState({
    email: "",
    senha: "",
    lembrarDeMim: false,
  })

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target
    setFormData((prev) => ({ ...prev, [name]: value }))
  }

  const handleCheckboxChange = (checked: boolean) => {
    setFormData((prev) => ({ ...prev, lembrarDeMim: checked }))
  }

  const resetForm = () => {
    setFormData({
      email: "",
      senha: "",
      lembrarDeMim: false,
    })
  }

const handleSubmit = async (e: React.FormEvent) => {
  e.preventDefault()
  setIsSubmitting(true)

  try {
    const response = await fetch("https://f940-177-131-178-100.ngrok-free.app/api/Account/login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
      },
      body: JSON.stringify({
        Email: formData.email,
        Password: formData.senha,
        RememberMe: formData.lembrarDeMim,
      }),
    })

    if (!response.ok) {
      const errorText = await response.text()
      toast({
        title: "Erro ao entrar",
        description: errorText,
        variant: "destructive",
        duration: 5000,
      })
      setIsSubmitting(false)
      return
    }

    const data = await response.json()

    // Salva token no localStorage
    localStorage.setItem("token", data.token)

    // Atualiza contexto de autenticação
    login(data.token)

    toast({
      title: "Login realizado com sucesso!",
      description: "Você será redirecionado para a página inicial.",
      duration: 5000,
    })

    resetForm()
    setTimeout(() => router.push("/"), 0) //delay após login
  } catch (error) {
    console.error("Erro ao fazer login:", error)
    toast({
      title: "Erro inesperado",
      description: "Não foi possível realizar o login.",
      variant: "destructive",
      duration: 5000,
    })
  } finally {
    setIsSubmitting(false)
  }
}


  return (
    <div className="min-h-screen flex flex-col bg-gray-50">
      <Navbar />
      <div className="flex-1 flex items-center justify-center p-4">
        <Card className="w-full max-w-md">
          <CardHeader>
            <CardTitle className="text-2xl">Entrar</CardTitle>
            <CardDescription>Acesse sua conta para gerenciar sua loja</CardDescription>
          </CardHeader>
          <CardContent>
            <form className="space-y-4" onSubmit={handleSubmit}>
              <div className="space-y-2">
                <Label htmlFor="email">Email</Label>
                <Input
                  id="email"
                  name="email"
                  type="email"
                  placeholder="seu.email@exemplo.com"
                  value={formData.email}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className="space-y-2">
                <div className="flex items-center justify-between">
                  <Label htmlFor="senha">Senha</Label>
                  <Link href="/recuperar-senha" className="text-sm text-primary hover:underline">
                    Esqueceu a senha?
                  </Link>
                </div>
                <Input
                  id="senha"
                  name="senha"
                  type="password"
                  placeholder="Digite sua senha"
                  value={formData.senha}
                  onChange={handleChange}
                  required
                />
              </div>
              <div className="flex items-center space-x-2">
                <Checkbox id="lembrarDeMim" checked={formData.lembrarDeMim} onCheckedChange={handleCheckboxChange} />
                <Label htmlFor="lembrarDeMim" className="text-sm font-normal">
                  Lembrar-se de mim
                </Label>
              </div>
              <Button type="submit" className="w-full mt-4" disabled={isSubmitting}>
                {isSubmitting ? "Entrando..." : "Entrar"}
              </Button>
            </form>
          </CardContent>
          <CardFooter className="flex justify-center">
            <div className="text-center text-sm">
              Não tem uma conta?{" "}
              <Link href="/cadastro" className="text-primary hover:underline">
                Cadastre-se
              </Link>
            </div>
          </CardFooter>
        </Card>
      </div>
    </div>
  )
}
