using System;

/*
 * Copyright Camunda Services GmbH and/or licensed to Camunda Services GmbH
 * under one or more contributor license agreements. See the NOTICE file
 * distributed with this work for additional information regarding copyright
 * ownership. Camunda licenses this file to you under the Apache License,
 * Version 2.0; you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace org.camunda.bpm.integrationtest.util
{
	using JavaArchive = org.jboss.shrinkwrap.api.spec.JavaArchive;
	using Maven = org.jboss.shrinkwrap.resolver.api.maven.Maven;
	using ScopeType = org.jboss.shrinkwrap.resolver.api.maven.ScopeType;
	using MavenDependencies = org.jboss.shrinkwrap.resolver.api.maven.coordinate.MavenDependencies;
	using RejectDependenciesStrategy = org.jboss.shrinkwrap.resolver.api.maven.strategy.RejectDependenciesStrategy;


	public class DeploymentHelper
	{

	  public const string CAMUNDA_EJB_CLIENT = "org.camunda.bpm.javaee:camunda-ejb-client";
	  public const string CAMUNDA_ENGINE_CDI = "org.camunda.bpm:camunda-engine-cdi";
	  public const string CAMUNDA_ENGINE_SPRING = "org.camunda.bpm:camunda-engine-spring";

	  private static JavaArchive CACHED_CLIENT_ASSET;
	  private static JavaArchive CACHED_ENGINE_CDI_ASSET;
	  private static JavaArchive[] CACHED_WELD_ASSETS;
	  private static JavaArchive[] CACHED_SPRING_ASSETS;

	  public static JavaArchive EjbClient
	  {
		  get
		  {
			if (CACHED_CLIENT_ASSET != null)
			{
			  return CACHED_CLIENT_ASSET;
			}
			else
			{
    
			  JavaArchive[] resolvedArchives = Maven.configureResolver().workOffline().loadPomFromFile("pom.xml").resolve(CAMUNDA_EJB_CLIENT).withTransitivity().@as(typeof(JavaArchive));
    
			  if (resolvedArchives.Length != 1)
			  {
				throw new Exception("could not resolve " + CAMUNDA_EJB_CLIENT);
			  }
			  else
			  {
				CACHED_CLIENT_ASSET = resolvedArchives[0];
				return CACHED_CLIENT_ASSET;
			  }
			}
    
		  }
	  }

	  public static JavaArchive EngineCdi
	  {
		  get
		  {
			if (CACHED_ENGINE_CDI_ASSET != null)
			{
			  return CACHED_ENGINE_CDI_ASSET;
			}
			else
			{
    
			  JavaArchive[] resolvedArchives = Maven.configureResolver().workOffline().loadPomFromFile("pom.xml").resolve(CAMUNDA_ENGINE_CDI).withTransitivity().@as(typeof(JavaArchive));
    
			  if (resolvedArchives.Length != 1)
			  {
				throw new Exception("could not resolve " + CAMUNDA_ENGINE_CDI);
			  }
			  else
			  {
				CACHED_ENGINE_CDI_ASSET = resolvedArchives[0];
				return CACHED_ENGINE_CDI_ASSET;
			  }
			}
		  }
	  }

	  public static JavaArchive[] Weld
	  {
		  get
		  {
			if (CACHED_WELD_ASSETS != null)
			{
			  return CACHED_WELD_ASSETS;
			}
			else
			{
    
			  JavaArchive[] resolvedArchives = Maven.configureResolver().workOffline().loadPomFromFile("pom.xml").resolve(CAMUNDA_ENGINE_CDI, "org.jboss.weld.servlet:weld-servlet").withTransitivity().@as(typeof(JavaArchive));
    
			  if (resolvedArchives.Length == 0)
			  {
				throw new Exception("could not resolve org.jboss.weld.servlet:weld-servlet");
			  }
			  else
			  {
				CACHED_WELD_ASSETS = resolvedArchives;
				return CACHED_WELD_ASSETS;
			  }
			}
    
		  }
	  }

	  public static JavaArchive[] EngineSpring
	  {
		  get
		  {
			if (CACHED_SPRING_ASSETS != null)
			{
			  return CACHED_SPRING_ASSETS;
			}
			else
			{
    
			  JavaArchive[] resolvedArchives = Maven.configureResolver().workOffline().loadPomFromFile("pom.xml").addDependencies(MavenDependencies.createDependency("org.camunda.bpm:camunda-engine-spring", ScopeType.COMPILE, false, MavenDependencies.createExclusion("org.camunda.bpm:camunda-engine")), MavenDependencies.createDependency("org.springframework:spring-context", ScopeType.COMPILE, false), MavenDependencies.createDependency("org.springframework:spring-jdbc", ScopeType.COMPILE, false), MavenDependencies.createDependency("org.springframework:spring-tx", ScopeType.COMPILE, false), MavenDependencies.createDependency("org.springframework:spring-orm", ScopeType.COMPILE, false), MavenDependencies.createDependency("org.springframework:spring-web", ScopeType.COMPILE, false)).resolve().withTransitivity().@as(typeof(JavaArchive));
    
			  if (resolvedArchives.Length == 0)
			  {
				throw new Exception("could not resolve org.camunda.bpm:camunda-engine-spring");
			  }
			  else
			  {
				CACHED_SPRING_ASSETS = resolvedArchives;
				return CACHED_SPRING_ASSETS;
			  }
			}
    
		  }
	  }

	  public static JavaArchive[] getJodaTimeModuleForServer(string server)
	  {
		if (server.Equals("tomcat") || server.Equals("websphere9") || server.Equals("weblogic") || server.Equals("glassfish"))
		{
		  return Maven.configureResolver().workOffline().loadPomFromFile("pom.xml").resolve("com.fasterxml.jackson.datatype:jackson-datatype-joda").@using(new RejectDependenciesStrategy(false, "joda-time:joda-time")).@as(typeof(JavaArchive));
		}
		else if (server.Equals("websphere"))
		{
		  return Maven.configureResolver().workOffline().loadPomFromFile("pom.xml", "was85").resolve("com.fasterxml.jackson.datatype:jackson-datatype-joda").@using(new RejectDependenciesStrategy(false, "joda-time:joda-time")).@as(typeof(JavaArchive));
		}
		else if (server.Equals("jboss"))
		{
		  return Maven.configureResolver().workOffline().loadPomFromFile("pom.xml").resolve("com.fasterxml.jackson.datatype:jackson-datatype-joda").@using(new RejectDependenciesStrategy(false, "com.fasterxml.jackson.core:jackson-annotations", "com.fasterxml.jackson.core:jackson-core", "com.fasterxml.jackson.core:jackson-databind")).@as(typeof(JavaArchive));
		}
		else
		{
		  throw new Exception("Unable to determine dependencies for jodaTimeModule: " + server);
		}
	  }

	  public static JavaArchive[] getSpinJacksonJsonDataFormatForServer(string server)
	  {
		if (server.Equals("tomcat") || server.Equals("websphere9") || server.Equals("weblogic") || server.Equals("glassfish"))
		{
		  return Maven.configureResolver().workOffline().loadPomFromFile("pom.xml").resolve("org.camunda.spin:camunda-spin-dataformat-json-jackson").@using(new RejectDependenciesStrategy(false, "org.camunda.spin:camunda-spin-core", "org.camunda.commons:camunda-commons-logging", "org.camunda.commons:camunda-commons-utils")).@as(typeof(JavaArchive));
		}
		else if (server.Equals("websphere"))
		{
		  return Maven.configureResolver().workOffline().loadPomFromFile("pom.xml", "was85").resolve("org.camunda.spin:camunda-spin-dataformat-json-jackson").@using(new RejectDependenciesStrategy(false, "org.camunda.spin:camunda-spin-core", "org.camunda.commons:camunda-commons-logging", "org.camunda.commons:camunda-commons-utils")).@as(typeof(JavaArchive));
		}
		else
		{
		  throw new Exception("Unable to determine dependencies for spinJacksonJsonDataFormat: " + server);
		}
	  }

	}

}