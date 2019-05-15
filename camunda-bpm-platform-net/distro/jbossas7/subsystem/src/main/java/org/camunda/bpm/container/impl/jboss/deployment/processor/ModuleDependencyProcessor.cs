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
namespace org.camunda.bpm.container.impl.jboss.deployment.processor
{
	using ProcessApplicationAttachments = org.camunda.bpm.container.impl.jboss.deployment.marker.ProcessApplicationAttachments;
	using ProcessApplicationModuleService = org.camunda.bpm.container.impl.jboss.service.ProcessApplicationModuleService;
	using ServiceNames = org.camunda.bpm.container.impl.jboss.service.ServiceNames;
	using AttachmentList = org.jboss.@as.server.deployment.AttachmentList;
	using Attachments = org.jboss.@as.server.deployment.Attachments;
	using DeploymentPhaseContext = org.jboss.@as.server.deployment.DeploymentPhaseContext;
	using DeploymentUnit = org.jboss.@as.server.deployment.DeploymentUnit;
	using DeploymentUnitProcessingException = org.jboss.@as.server.deployment.DeploymentUnitProcessingException;
	using DeploymentUnitProcessor = org.jboss.@as.server.deployment.DeploymentUnitProcessor;
	using ModuleDependency = org.jboss.@as.server.deployment.module.ModuleDependency;
	using ModuleSpecification = org.jboss.@as.server.deployment.module.ModuleSpecification;
	using Module = org.jboss.modules.Module;
	using ModuleIdentifier = org.jboss.modules.ModuleIdentifier;
	using ModuleLoader = org.jboss.modules.ModuleLoader;
	using Mode = org.jboss.msc.service.ServiceController.Mode;
	using ServiceName = org.jboss.msc.service.ServiceName;


	/// <summary>
	/// <para>This Processor creates implicit module dependencies for process applications</para>
	/// 
	/// <para>Concretely speaking, this processor adds a module dependency from the process
	/// application module (deployment unit) to the process engine module (and other camunda libraries
	/// which are useful for process apps).</para>
	/// 
	/// @author Daniel Meyer
	/// 
	/// </summary>
	public class ModuleDependencyProcessor : DeploymentUnitProcessor
	{

	  public const int PRIORITY = 0x2300;

	  public static ModuleIdentifier MODULE_IDENTIFYER_PROCESS_ENGINE = ModuleIdentifier.create("org.camunda.bpm.camunda-engine");
	  public static ModuleIdentifier MODULE_IDENTIFYER_XML_MODEL = ModuleIdentifier.create("org.camunda.bpm.model.camunda-xml-model");
	  public static ModuleIdentifier MODULE_IDENTIFYER_BPMN_MODEL = ModuleIdentifier.create("org.camunda.bpm.model.camunda-bpmn-model");
	  public static ModuleIdentifier MODULE_IDENTIFYER_CMMN_MODEL = ModuleIdentifier.create("org.camunda.bpm.model.camunda-cmmn-model");
	  public static ModuleIdentifier MODULE_IDENTIFYER_DMN_MODEL = ModuleIdentifier.create("org.camunda.bpm.model.camunda-dmn-model");
	  public static ModuleIdentifier MODULE_IDENTIFYER_SPIN = ModuleIdentifier.create("org.camunda.spin.camunda-spin-core");
	  public static ModuleIdentifier MODULE_IDENTIFYER_CONNECT = ModuleIdentifier.create("org.camunda.connect.camunda-connect-core");
	  public static ModuleIdentifier MODULE_IDENTIFYER_ENGINE_DMN = ModuleIdentifier.create("org.camunda.bpm.dmn.camunda-engine-dmn");

//JAVA TO C# CONVERTER WARNING: Method 'throws' clauses are not available in .NET:
//ORIGINAL LINE: public void deploy(org.jboss.as.server.deployment.DeploymentPhaseContext phaseContext) throws org.jboss.as.server.deployment.DeploymentUnitProcessingException
	  public virtual void deploy(DeploymentPhaseContext phaseContext)
	  {

//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.as.server.deployment.DeploymentUnit deploymentUnit = phaseContext.getDeploymentUnit();
		DeploymentUnit deploymentUnit = phaseContext.DeploymentUnit;

		if (deploymentUnit.Parent == null)
		{
		  //The deployment unit has no parent so it is a simple war or an ear.
		  ModuleLoader moduleLoader = Module.BootModuleLoader;
		  //If it is a simpleWar and marked with process application we have to add the dependency
		  bool isProcessApplicationWarOrEar = ProcessApplicationAttachments.isProcessApplication(deploymentUnit);

		  AttachmentList<DeploymentUnit> subdeployments = deploymentUnit.getAttachment(Attachments.SUB_DEPLOYMENTS);
		  //Is the list of sub deployments empty the deployment unit is a war file.
		  //In cases of war files we have nothing todo.
		  if (subdeployments != null)
		  {
			//The deployment unit contains sub deployments which means the deployment unit is an ear.
			//We have to check whether sub deployments are process applications or not.
			bool subDeploymentIsProcessApplication = false;
			foreach (DeploymentUnit subDeploymentUnit in subdeployments)
			{
			  if (ProcessApplicationAttachments.isProcessApplication(subDeploymentUnit))
			  {
				subDeploymentIsProcessApplication = true;
				break;
			  }
			}
			//If one sub deployment is a process application then we add to all the dependency
			//Also we have to add the dependency to the current deployment unit which is an ear
			if (subDeploymentIsProcessApplication)
			{
			  foreach (DeploymentUnit subDeploymentUnit in subdeployments)
			  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.as.server.deployment.module.ModuleSpecification moduleSpecification = subDeploymentUnit.getAttachment(org.jboss.as.server.deployment.Attachments.MODULE_SPECIFICATION);
				ModuleSpecification moduleSpecification = subDeploymentUnit.getAttachment(Attachments.MODULE_SPECIFICATION);
				addSystemDependencies(moduleLoader, moduleSpecification);
			  }
			  //An ear is not marked as process application but also needs the dependency
			  isProcessApplicationWarOrEar = true;
			}
		  }

		  if (isProcessApplicationWarOrEar)
		  {
//JAVA TO C# CONVERTER WARNING: The original Java variable was marked 'final':
//ORIGINAL LINE: final org.jboss.as.server.deployment.module.ModuleSpecification moduleSpecification = deploymentUnit.getAttachment(org.jboss.as.server.deployment.Attachments.MODULE_SPECIFICATION);
			ModuleSpecification moduleSpecification = deploymentUnit.getAttachment(Attachments.MODULE_SPECIFICATION);
			addSystemDependencies(moduleLoader, moduleSpecification);
		  }
		}

		// install the pa-module service
		ModuleIdentifier identifyer = deploymentUnit.getAttachment(Attachments.MODULE_IDENTIFIER);
		string moduleName = identifyer.ToString();

		ProcessApplicationModuleService processApplicationModuleService = new ProcessApplicationModuleService();
		ServiceName serviceName = ServiceNames.forProcessApplicationModuleService(moduleName);

		phaseContext.ServiceTarget.addService(serviceName, processApplicationModuleService).addDependency(phaseContext.PhaseServiceName).setInitialMode(Mode.ACTIVE).install();

	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void addSystemDependencies(org.jboss.modules.ModuleLoader moduleLoader, final org.jboss.as.server.deployment.module.ModuleSpecification moduleSpecification)
	  private void addSystemDependencies(ModuleLoader moduleLoader, ModuleSpecification moduleSpecification)
	  {
		addSystemDependency(moduleLoader, moduleSpecification, MODULE_IDENTIFYER_PROCESS_ENGINE);
		addSystemDependency(moduleLoader, moduleSpecification, MODULE_IDENTIFYER_XML_MODEL);
		addSystemDependency(moduleLoader, moduleSpecification, MODULE_IDENTIFYER_BPMN_MODEL);
		addSystemDependency(moduleLoader, moduleSpecification, MODULE_IDENTIFYER_CMMN_MODEL);
		addSystemDependency(moduleLoader, moduleSpecification, MODULE_IDENTIFYER_DMN_MODEL);
		addSystemDependency(moduleLoader, moduleSpecification, MODULE_IDENTIFYER_SPIN);
		addSystemDependency(moduleLoader, moduleSpecification, MODULE_IDENTIFYER_CONNECT);
		addSystemDependency(moduleLoader, moduleSpecification, MODULE_IDENTIFYER_ENGINE_DMN);
	  }

//JAVA TO C# CONVERTER WARNING: 'final' parameters are not available in .NET:
//ORIGINAL LINE: private void addSystemDependency(org.jboss.modules.ModuleLoader moduleLoader, final org.jboss.as.server.deployment.module.ModuleSpecification moduleSpecification, org.jboss.modules.ModuleIdentifier dependency)
	  private void addSystemDependency(ModuleLoader moduleLoader, ModuleSpecification moduleSpecification, ModuleIdentifier dependency)
	  {
		moduleSpecification.addSystemDependency(new ModuleDependency(moduleLoader, dependency, false, false, false, false));
	  }

	  public virtual void undeploy(DeploymentUnit context)
	  {

	  }

	}

}