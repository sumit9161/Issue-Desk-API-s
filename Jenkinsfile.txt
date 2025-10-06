pipeline {
    agent any

    stages {
        stage('Checkout') {
            steps {
                git branch: 'main', url: 'https://github.com/sumit9161/Issue-Desk-API-s.git'
            }
        }

        stage('Restore') {
            steps {
                bat 'dotnet restore'
            }
        }

        stage('Build') {
            steps {
                bat 'dotnet build --configuration Release'
            }
        }

        stage('Publish') {
            steps {
                bat 'dotnet publish --configuration Release -o published'
            }
        }
    }

    post {
        success {
            echo '✅ Build & publish succeeded!'
        }
        failure {
            echo '❌ Build failed!'
        }
    }
}
