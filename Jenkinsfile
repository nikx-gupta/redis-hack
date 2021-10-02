pipeline {
  agent any
  stages {
    stage('Build Image') {
      steps {
        sh 'git submodule update --init --recursive'
        sh 'docker build -t mongo-app -f dotnet/Dockerfile dotnet/'
      }
    }

    stage('Deploy') {
      steps {
        sh 'docker run --name mongo-app -p 9020:80 -d -e ASPNETCORE_ENVIRONMENT=DEV mongo-app'
      }
    }

  }
}