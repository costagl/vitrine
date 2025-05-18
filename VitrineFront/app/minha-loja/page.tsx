"use client"

import { useEffect } from "react"
import { useRouter } from "next/navigation"
import { useAuth } from "@/contexts/auth-context"
import Navbar from "@/components/navbar"

export default function MinhaLojaPage() {
  const { isAuthenticated } = useAuth()
  const router = useRouter()

  useEffect(() => {
    // Redirecionar para login se não estiver autenticado
    if (!isAuthenticated) {
      router.push("/login")
    }
  }, [isAuthenticated, router])

  const testarAPI = async () => {
    try {
      const resposta = await fetch("https://f940-177-131-178-100.ngrok-free.app/api/Account/test")

      if (resposta.ok) {
        console.log("✅ API funcionando!")
      } else {
        console.log("❌ API respondeu com erro:", resposta.status)
      }
    } catch (error) {
      console.log("❌ Erro ao conectar com a API:", error.message)
    }
  }

  // Se não estiver autenticado, não renderiza o conteúdo
  if (!isAuthenticated) {
    return null
  }

  return (
    <div className="min-h-screen flex flex-col bg-gray-50">
      <Navbar />
      <div className="container mx-auto px-4 py-8">
        <h1 className="text-3xl font-bold mb-6">Minha Loja</h1>

        <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
          {/* Painel de Estatísticas */}
          <div className="bg-white p-6 rounded-lg shadow-md">
            <h2 className="text-xl font-semibold mb-4">Estatísticas</h2>
            <div className="space-y-4">
              <div>
                <p className="text-sm text-gray-500">Visitas hoje</p>
                <p className="text-2xl font-bold">127</p>
              </div>
              <div>
                <p className="text-sm text-gray-500">Vendas hoje</p>
                <p className="text-2xl font-bold">12</p>
              </div>
              <div>
                <p className="text-sm text-gray-500">Receita hoje</p>
                <p className="text-2xl font-bold">R$ 1.247,00</p>
              </div>
            </div>
          </div>

          {/* Pedidos Recentes */}
          <div className="bg-white p-6 rounded-lg shadow-md">
            <h2 className="text-xl font-semibold mb-4">Pedidos Recentes</h2>
            <div className="space-y-3">
              <div className="flex justify-between items-center pb-2 border-b">
                <div>
                  <p className="font-medium">#12345</p>
                  <p className="text-sm text-gray-500">Há 2 horas</p>
                </div>
                <span className="px-2 py-1 bg-green-100 text-green-800 text-xs rounded-full">Pago</span>
              </div>
              <div className="flex justify-between items-center pb-2 border-b">
                <div>
                  <p className="font-medium">#12344</p>
                  <p className="text-sm text-gray-500">Há 3 horas</p>
                </div>
                <span className="px-2 py-1 bg-yellow-100 text-yellow-800 text-xs rounded-full">Pendente</span>
              </div>
              <div className="flex justify-between items-center pb-2 border-b">
                <div>
                  <p className="font-medium">#12343</p>
                  <p className="text-sm text-gray-500">Há 5 horas</p>
                </div>
                <span className="px-2 py-1 bg-blue-100 text-blue-800 text-xs rounded-full">Enviado</span>
              </div>
            </div>
          </div>

          {/* Ações Rápidas */}
          <div className="bg-white p-6 rounded-lg shadow-md">
            <h2 className="text-xl font-semibold mb-4">Ações Rápidas</h2>
            <div className="space-y-3">
              <button className="w-full py-2 px-4 bg-primary text-white rounded-md hover:bg-primary/90 transition-colors">
                Adicionar Produto
              </button>
              <button className="w-full py-2 px-4 border border-gray-300 rounded-md hover:bg-gray-50 transition-colors">
                Ver Pedidos
              </button>
              <button className="w-full py-2 px-4 border border-gray-300 rounded-md hover:bg-gray-50 transition-colors">
                Configurações da Loja
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}
